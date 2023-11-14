using Microsoft.VisualBasic.ApplicationServices;
using Models.Common;
using Models.Users;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Tools;
using User = Models.Users.User;

namespace BL
{
    public class UserBl
    {
        EventBl eventBl = new EventBl();
        Credentials cred = new Credentials();

        PrincipalContext context = new PrincipalContext(ContextType.Machine);

        #region CRUD
        public void CreateUser(User user)
        {
            //using(var imp = new Impersonator(cred.UserName, cred.Password))
            //{
            UserPrincipal principal = new UserPrincipal(context);
            principal.DisplayName = user.FullName;
            principal.SamAccountName = user.UserName;
            principal.SetPassword("123");
            principal.Enabled = !user.Enabled;
            principal.PasswordNeverExpires = true;
            if (user.ChangePassNextTime)
            {
                principal.ExpirePasswordNow();
            }
            principal.Save();

            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, Credentials.VpnClientsGroup);
            if (group != null)
            {
                group.Members.Add(principal);
                group.Save();
            }
            UpdateExtraInfoDb();
            //}
        }
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            var userPrincipal = new UserPrincipal(context);
            var searcher = new PrincipalSearcher(userPrincipal);
            var results = searcher.FindAll();

            foreach (var result in results)
            {
                users.Add(UserPrincipalToUser((UserPrincipal)result));
            }
            return users;
        }
        public User GetUser(string username)
        {
            var principal = new UserPrincipal(context) { SamAccountName = username };
            var searcher = new PrincipalSearcher(principal);
            var search = searcher.FindOne();

            if (search == null) throw new Exception("Account not found!");
            else return UserPrincipalToUser((UserPrincipal)search);
        }
        public void UpdateUser(UserProfileVm user, bool change_pass, bool IsAdminAsking = false)
        {

            if (user.ExtraInfo != null)
            {
                UpdateUserExtraInfo(user.ExtraInfo);
            }

            //using (var imp = new Impersonator(cred.UserName, cred.Password))
            //{
            UserPrincipal search = UserPrincipal.FindByIdentity(context, IdentityType.Sid, user.SID);

            if (search == null)
            {
                throw new Exception("Account not found!");
            }
            else
            {
                if (IsAdminAsking || change_pass)
                {
                    if (!string.IsNullOrEmpty(user.password) || !string.IsNullOrEmpty(user.confirm_password))
                    {
                        if (user.password != user.confirm_password)
                        {
                            throw new Exception("Password and the password confirm must be the same!");
                        }
                        else
                        {
                            search.SetPassword(user.password);
                        }
                    }
                }
                search.Name = user.FullName;
                search.DisplayName = user.FullName;
                if (IsAdminAsking)
                {
                    search.Enabled = user.Enabled;
                    search.PasswordNeverExpires = !user.ChangePassNextTime;
                    search.UserCannotChangePassword = false;
                    if (user.ChangePassNextTime) search.ExpirePasswordNow();
                }
                search.Save();

                if (IsAdminAsking)
                {
                    using (var de = search.GetUnderlyingObject() as DirectoryEntry)
                    {
                        de.RefreshCache();
                        var parameters = de.Properties["Parameters"];
                        de.Properties["Parameters"].Value = "";
                        if (!string.IsNullOrEmpty(user.ReservedIpAddress))
                        {
                            var newParameters = Ip.update_dialin_parameters(parameters.Value.ToString(), user.ReservedIpAddress);
                            de.Properties["Parameters"].Value = newParameters;
                        }
                        de.CommitChanges();
                    }
                }   
                //}

            }
        }
        public void DeleteUser(string username)
        {
            //using(var imp = new Impersonator(cred.UserName, cred.Password))
            //{
            UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
            if (user != null)
            {
                user.Delete();
            }
            //}
        }
        public List<string> GetGroupsForUser(UserPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var list_groups = new List<string>();

            var groups = (user).GetGroups();

            foreach (var group in groups)
            {
                list_groups.Add(group.Name);
            }
            return list_groups;
        }
        public User UserPrincipalToUser(UserPrincipal _user)
        {
            if (_user == null) throw new Exception("Account not found!");
            else
            {
                var name = "";
                if (_user.DisplayName != null)
                    name = _user.DisplayName.ToString();
                var username = _user.SamAccountName.ToString();

                var de = _user.GetUnderlyingObject() as DirectoryEntry;
                de.RefreshCache();
                var parameters = de.Properties["Parameters"];
                var ipAddr = "";
                if (parameters != null && parameters.Count > 0 && !string.IsNullOrEmpty(parameters.Value.ToString()))
                {
                    try { ipAddr = Ip.get_ip_from_dialin_parameters(parameters.Value.ToString()); }
                    catch (Exception) { ipAddr = "-1"; }
                }

                return new User()
                {
                    SID = _user.Sid.ToString(),
                    UserName = username,
                    FullName = name,
                    Enabled = (bool)_user.Enabled,
                    ChangePassNextTime = !_user.PasswordNeverExpires,
                    ExtraInfo = getUsersExtraInfo(username).FirstOrDefault(),
                    Groups = GetGroupsForUser(_user),
                    ReservedIpAddress = ipAddr
                };
            }
        }
        public List<string> GetUsersForGroup(string groupName)
        {
            List<string> usernames = new List<string>();

            var groupPrincipal = GroupPrincipal.FindByIdentity(context, groupName);

            if (groupPrincipal != null)
            {
                var members = groupPrincipal.GetMembers();

                foreach (var member in members)
                {
                    if (member is UserPrincipal userPrincipal)
                    {
                        usernames.Add(userPrincipal.SamAccountName);
                    }
                }
            }

            return usernames;
        }
        #endregion

        #region Custom Update/Read
        public bool CheckAdminLogin(string username, string password)
        {
            if (context.ValidateCredentials(username, password))
            {
                using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username))
                {
                    return user.IsMemberOf(context, IdentityType.Name, "Administrators") ? true : throw new Exception("User is not an administrator");
                }
            }
            else
            {
                throw new Exception("Invalid username or password");
            }   
        }
        public bool? CheckClientLogin(string username, string password)
        {
            if (context.ValidateCredentials(username, password))
            {
                using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username))
                {
                    return user.IsMemberOf(context, IdentityType.Name, Credentials.VpnClientsGroup) ? true : throw new Exception("User is not a VPN User!");
                }
            }
            else
            {
                throw new Exception("Invalid username or password");
            }
        }
        public void ChangePasswordAndUnlock(ChangePassVm user_obj)
        {
            if (!string.IsNullOrEmpty(user_obj.NewPassword) && !string.IsNullOrEmpty(user_obj.ConfirmPassword) &&
                user_obj.NewPassword.Length > 0 && user_obj.ConfirmPassword.Length > 0 && user_obj.NewPassword == user_obj.ConfirmPassword)
            {
                //using(var imp = new Impersonator(cred.UserName, cred.Password))
                //{
                UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, user_obj.Username);
                if (user != null)
                {
                    user.SetPassword(user_obj.NewPassword);
                    user.UnlockAccount();
                    user.PasswordNeverExpires = true;
                    user.Enabled = true;
                    user.Save();
                }
                //}
            }
            else
            {
                throw new Exception("Wrong Values!");
            }
        }
        public void EnableUser(string username)
        {
            //using (var imp = new Impersonator(cred.UserName, cred.Password))
            //{
            UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
            if (user != null)
            {
                user.Enabled = true;
                user.Save();
            }
            //}
        }
        public void DisableUser(string username)
        {
            //using (var imp = new Impersonator(cred.UserName, cred.Password))
            //{
            UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
            if (user != null)
            {
                user.Enabled = false;
                user.Save();
            }
            //}
        }
        public void AddToGroup(string username, string groupname)
        {
            RunCommand.run($"net localgroup \"{groupname}\" {username} /add");
        }
        public void RemoveFromGroup(string username, string groupname)
        {
            RunCommand.run($"net localgroup \"{groupname}\" {username} /delete");
        }
        public List<User> GetIpReservations()
        {
            var res = GetUsers().ToList().Where(u => u.Groups.Contains(Credentials.VpnClientsGroup)).ToList();
            res.Sort((u1, u2) => string.Compare(u1.UserName, u2.UserName));
            return res;
        }
        public static string ConvertStringToCharCodes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return string.Join("-", input.Select(c => ((int)c).ToString()));
        }

        public void UpdateIpReservations(List<User> users)
        {
            //using (var imp = new Impersonator(cred.UserName, cred.Password))
            //{
                string[] attributesToLoad = { "samAccountName", "Parameters" };
                string basePath = $"WinNT://WORKGROUP/{cred.PublicAddress}";
                foreach (var user in users)
                {
                    string userPath = $"{basePath}/{user.UserName}";
                    DirectoryEntry directoryEntry = new DirectoryEntry(userPath);
                    directoryEntry.RefreshCache();
                    var parameters = directoryEntry.Properties["Parameters"];
                    string prevIP = "";
                    if (parameters != null && parameters.Count > 0 && !string.IsNullOrEmpty(parameters.Value.ToString()))
                    {
                        try { prevIP = Ip.get_ip_from_dialin_parameters(parameters.Value.ToString()); }
                        catch (Exception) { prevIP = ""; }
                    }
                    if ( !(string.IsNullOrEmpty(prevIP) && string.IsNullOrEmpty(user.ReservedIpAddress)) &&
                        !string.Equals(prevIP, user.ReservedIpAddress))
                    {
                        directoryEntry.Properties["Parameters"].Value = "";
                        if (!string.IsNullOrEmpty(user.ReservedIpAddress))
                            directoryEntry.Properties["Parameters"].Value = Ip.update_dialin_parameters("", user.ReservedIpAddress);
                        directoryEntry.CommitChanges();
                    }
                }
            //}
        }
        #endregion

        #region RRAS Related
        public void UpdateExtraInfoDb()
        {
            List<UserExtraInfo> info = null;
            XmlSerializer serializer = new XmlSerializer(typeof(List<UserExtraInfo>));
            if (!File.Exists(Credentials.UsersExtraInfoFilePath))
            {
                info = new List<UserExtraInfo>();

                List<User> users = new List<User>();
                var userPrincipal = new UserPrincipal(context);
                var searcher = new PrincipalSearcher(userPrincipal);
                var results = searcher.FindAll();

                foreach (var result in results)
                {
                    var _user = (UserPrincipal)result;
                    var username = _user.SamAccountName.ToString();
                    var groups = GetGroupsForUser(_user);
                    if (groups.Contains(Credentials.VpnClientsGroup))
                    {
                        info.Add(new UserExtraInfo()
                        {
                            Username = username,
                            MultilinkCapacity = 1
                        });
                    }
                }
                
                //for (int i = 0; i < users.Count(); i++)
                //{
                //    if (users[i].Groups.Contains(Credentials.VpnClientsGroup)) 
                //    { 
                //        info.Add(new UserExtraInfo()
                //        {
                //            Username = users[i].UserName,
                //            MultilinkCapacity = 1
                //        });
                //    }
                //}
                using (TextWriter writer = new StreamWriter(Credentials.UsersExtraInfoFilePath))
                {
                    serializer.Serialize(writer, info);
                }
            }
            else
            {
                StreamReader file = new StreamReader(Credentials.UsersExtraInfoFilePath);
                info = (List<UserExtraInfo>)serializer.Deserialize(file);
                var new_info = new List<UserExtraInfo>();
                file.Close();
                var users = GetUsers();
                for (int i = 0; i < users.Count(); i++)
                {
                    //var username = users[i].Properties["cn"][0].ToString().ToLower();
                    var username = users[i].UserName;
                    var groups = new List<string>();
                    //users[i].Properties["memberof"].Cast<string>().ToList().ForEach(sr => groups.Add(sr.Split(',')[0].Replace("CN=", "")));
                    //bool shouldBe = groups.Contains(Credentials.VpnClientsGroup);
                    bool shouldBe = users[i].Groups.Contains(Credentials.VpnClientsGroup);
                    var get_info = info.Where(u => u.Username == username);
                    if (shouldBe)
                    {
                        new_info.Add(new UserExtraInfo()
                        {
                            Username = username,
                            MultilinkCapacity = !get_info.Any() ? 1 : get_info.First().MultilinkCapacity
                        });
                    }
                }
                File.Delete(Credentials.UsersExtraInfoFilePath);
                using (TextWriter writer = new StreamWriter(Credentials.UsersExtraInfoFilePath))
                {
                    serializer.Serialize(writer, new_info);
                }
            }
        }
        public List<UserExtraInfo> getUsersExtraInfo(string username = null)
        {
            List<UserExtraInfo> info = null;
            XmlSerializer serializer = new XmlSerializer(typeof(List<UserExtraInfo>));
            if (File.Exists(Credentials.UsersExtraInfoFilePath))
            {
                StreamReader file = new StreamReader(Credentials.UsersExtraInfoFilePath);
                info = (List<UserExtraInfo>)serializer.Deserialize(file);
                file.Close();
            }
            else
            {
                UpdateExtraInfoDb();
            }
            if(username == null)
            {
                return info;
            }
            else
            {
                return new List<UserExtraInfo>() { 
                    info.FirstOrDefault(i => i.Username == username.ToLower()) };
            }
        }
        public void UpdateUserExtraInfo(UserExtraInfo info)
        {
            if (!File.Exists(Credentials.UsersExtraInfoFilePath))
            {
                throw new Exception("Users extra info file is not created!");
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(Credentials.UsersExtraInfoFilePath);
            XmlNodeList selectUser = doc.SelectNodes(string.Format("/ArrayOfUserExtraInfo/UserExtraInfo[Username = '{0}']", info.Username.ToLower()));
            if (selectUser.Count > 0)
            {
                var optionToEdit = selectUser[0].SelectSingleNode("MultilinkCapacity");
                optionToEdit.InnerText = info.MultilinkCapacity.ToString();
            }
            doc.Save(Credentials.UsersExtraInfoFilePath);
        }
        public void CloseFalseConnection(string username, string connectionId)
        {
            eventBl.CreateLog("System", 20272, 0, EventLogEntryType.FailureAudit, new string[] { "Connection_Abortion", username, connectionId });
        }
        public void AnalyisisLogs(ref List<EventRecord> logs, ref Dictionary<string, Tuple<User, Dictionary<string, ConnectionInfo>, Dictionary<string, ConnectionInfo>>> data)
        {
            for (int i = 0; i < logs.Count(); i++)
            {
                var log = logs[i];
                // connection
                if (log.Id == 20274)
                {
                    if (!string.IsNullOrWhiteSpace(log.Properties[2].Value.ToString()))
                    {
                        var username = "";
                        if (log.Properties[2].Value.ToString().Contains('\\'))
                        {
                            username = log.Properties[2].Value.ToString().ToLower().Split('\\')[1];
                        }
                        else
                        {
                            username = log.Properties[2].Value.ToString().ToLower();
                        }
                        // add new connection
                        if (data.TryGetValue(username, out var userData))
                        {
                            string conn_id = log.Properties[1].Value.ToString();
                            var instance = new ConnectionInfo()
                            {
                                ConnectionId = conn_id,
                                UserName = username,
                                Port = log.Properties[3].Value.ToString(),
                                Ip = log.Properties[4].Value.ToString(),
                                ConnectionStartDate = (DateTime)log.TimeCreated,
                                ConnectionEndDate = null,
                                ConnectionDuration = DateTime.Now - (DateTime)log.TimeCreated,
                                Upload = InfoTransferUnit.ParseBytes(0),
                                Download = InfoTransferUnit.ParseBytes(0)
                            };
                            data[username].Item2[conn_id] = instance;
                            data[username].Item3[conn_id] = instance;
                        }
                    }
                }
                else if (log.Id == 20272)
                {
                    if (log.Properties[0].Value.ToString() == "Connection_Abortion")
                    {
                        var username = log.Properties[1].Value.ToString();
                        var conn_id = log.Properties[2].Value.ToString();
                        if (data.TryGetValue(username, out var userData))
                        {
                            if (data[username].Item2.TryGetValue(conn_id, out var connData2))
                            {
                                data[username].Item2[conn_id].ConnectionEndDate = data[username].Item2[conn_id].ConnectionStartDate;
                                data[username].Item2[conn_id].ConnectionDuration = TimeSpan.Zero;
                            }
                            if (data[username].Item3.TryGetValue(conn_id, out var connData3))
                            {
                                data[username].Item3.Remove(conn_id);
                            }
                        }
                    }
                    else if (log.Properties.Count >= 2 && !string.IsNullOrWhiteSpace(log.Properties[2].Value.ToString()))
                    {
                        var username = "";
                        if (log.Properties[2].Value.ToString().Contains('\\'))
                        {
                            username = log.Properties[2].Value.ToString().ToLower().Split('\\')[1];
                        }
                        else
                        {
                            username = log.Properties[2].Value.ToString().ToLower();
                        }
                        if (data.TryGetValue(username, out var userData))
                        {
                            string conn_id = log.Properties[1].Value.ToString();
                            // add new connection
                            if (data[username].Item2.TryGetValue(conn_id, out var connData2))
                            {
                                if (data[username].Item2[conn_id].Port == log.Properties[3].Value.ToString())
                                {
                                    data[username].Item2[conn_id].ConnectionEndDate = log.TimeCreated;
                                    data[username].Item2[conn_id].ConnectionDuration = (TimeSpan)(log.TimeCreated - data[username].Item2[conn_id].ConnectionStartDate);
                                    data[username].Item2[conn_id].Upload = InfoTransferUnit.ParseBytes(log.Properties[11].Value.ToString());
                                    data[username].Item2[conn_id].Download = InfoTransferUnit.ParseBytes(log.Properties[10].Value.ToString());
                                    data[username].Item3.Remove(conn_id);
                                }
                            }
                        }
                    }
                    else
                    {
                        int a = 0;
                    }
                }
            }
        }
        public async Task<List<UserLogVm>> GetUsersLogs()
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            var userAccounts = GetUsers();
            var data = new Dictionary<string, Tuple<User, Dictionary<string, ConnectionInfo>, Dictionary<string, ConnectionInfo>>>();
            for (int i = 0; i < userAccounts.Count(); i++)
            {
                var user = userAccounts.ElementAt(i);
                var user_name = user.UserName;
                var groups = user.Groups;

                if (user.Groups.Contains(Credentials.VpnClientsGroup))
                {
                    data[user_name] = new Tuple<User, Dictionary<string, ConnectionInfo>, Dictionary<string, ConnectionInfo>>(
                        new User()
                        {
                            UserName = user_name,
                            SID = user.SID,
                            ChangePassNextTime = user.ChangePassNextTime,
                            Enabled = user.Enabled,
                            FullConnection = user.Groups.Contains(Credentials.VpnClientsBlockGroup)
                        },
                        new Dictionary<string, ConnectionInfo>() { },
                        new Dictionary<string, ConnectionInfo>() { });
                }
            }

            var abortions = new List<Tuple<string, string>>();
            var logs = eventBl.ReadLogs("System", new int[2] { 20272, 20274 }).ToList();
            string username = string.Empty;

            await Task.Run(() => AnalyisisLogs(ref logs, ref data));

            var extraInfo = getUsersExtraInfo();

            var users = new List<UserLogVm>();
            foreach (var item in data)
            {
                var instance = new UserLogVm()
                {
                    UserName = item.Value.Item1.UserName,
                    SID = item.Value.Item1.SID,
                    Enabled = item.Value.Item1.Enabled,
                    ChangePassNextTime = item.Value.Item1.ChangePassNextTime,
                    FirstLog = DateTime.Now,
                    FullConnection = item.Value.Item1.FullConnection
                };

                var connections = item.Value.Item2.Values;
                var active_connections = item.Value.Item3.Values;
                var total_duration_Ticks = (from conn in connections select conn.ConnectionDuration).Sum(r => r.Ticks);
                var total_duration = (new TimeSpan(total_duration_Ticks));
                var total_download = (from conn in connections select conn.Download.TotalB).Sum();

                instance.Total_Download = InfoTransferUnit.ParseBytes(total_download);
                instance.Total_Duration = total_duration.ToFriendlyString(shortMode: 1);
                instance.Connections_Count = connections.Count();
                instance.Status = active_connections.Count() != 0;
                if (instance.Status)
                {
                    instance.Active_Duration = new TimeSpan((from conn in active_connections select conn.ConnectionDuration.Ticks).Max()).ToFriendlyString(shortMode: 1);
                }
                if (item.Value.Item2.Values.Count() > 0)
                {
                    instance.FirstLog = new DateTime((from conn in item.Value.Item2.Values select conn.ConnectionStartDate.Ticks).Min());
                }

                users.Add(instance);
            }
            return users.OrderByDescending(u => u.Total_Download.TotalB).ToList();
        }
        public User GetUserLogs(string _username)
        {
            var username = _username.Trim().ToLower();
            var user = GetUser(username);
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var data = new Dictionary<string, Tuple<User, Dictionary<string, ConnectionInfo>, Dictionary<string, ConnectionInfo>>>();
            data[username] = new Tuple<User, Dictionary<string, ConnectionInfo>, Dictionary<string, ConnectionInfo>>(
                        new User()
                        {
                            UserName = username,
                            FullName = user.FullName,
                            SID = user.SID,
                            Enabled = user.Enabled,
                            Groups = user.Groups,
                            ReservedIpAddress = user.ReservedIpAddress,
                        },
                        new Dictionary<string, ConnectionInfo>() { },
                        new Dictionary<string, ConnectionInfo>() { });

            var logs = eventBl.ReadLogs("System", new int[2] { 20272, 20274 }).ToList();

            AnalyisisLogs(ref logs, ref data);

            foreach (var item in data)
            {
                user = item.Value.Item1;
                user.ExtraInfo = getUsersExtraInfo().Where(i => i.Username == username).FirstOrDefault();
                user.FullConnection = user.Groups.Where(g => g.Contains(Credentials.VpnClientsBlockGroup)).Any();

                user.Connections = new List<ConnectionInfo>();
                user.ConnectionsCount = 0;
                user.ActiveConnections = new List<ConnectionInfo>();

                if (data.ContainsKey(username))
                {
                    if (data[username].Item2 != null)
                    {
                        user.Connections = data[username].Item2.Values.ToList();
                        user.ConnectionsCount = user.Connections.Count();
                    }
                    if (data[username].Item3 != null)
                    {
                        user.ActiveConnections = data[username].Item3.Values.ToList();
                    }
                }


                long totalBytesDown = (from conn in user.Connections select conn.Download.TotalB).Sum();
                long totalBytesUp = (from conn in user.Connections select conn.Upload.TotalB).Sum();
                long totalTicks = (from conn in user.Connections select conn.ConnectionDuration).Sum(r => r.Ticks);

                user.TotalDownload = InfoTransferUnit.ParseBytes(totalBytesDown);
                user.AverageDownloadEachTime = user.ConnectionsCount != 0 ? InfoTransferUnit.ParseBytes(totalBytesDown / user.ConnectionsCount) :
                    InfoTransferUnit.ParseBytes(0);

                user.TotalUpload = InfoTransferUnit.ParseBytes(totalBytesUp);
                user.AverageUploadEachTime = user.ConnectionsCount != 0 ? InfoTransferUnit.ParseBytes(totalBytesUp / user.ConnectionsCount) :
                    InfoTransferUnit.ParseBytes(0);

                user.TotalConnectionTime = new TimeSpan(totalTicks);
                user.AverageConnectionTime = user.ConnectionsCount != 0 ? new TimeSpan(totalTicks / user.ConnectionsCount) : TimeSpan.Zero;
            }
            return user;
        }
        public Dictionary<DateTime, List<ConnectionInfo>> GetPagedConnections(List<ConnectionInfo> connections, int pageNumber, int pageSize)
        {
            var days = new Dictionary<DateTime, List<ConnectionInfo>>();

            for (int i = connections.Count() - 1; i >= 0; i--)
            {
                var startDate = connections[i].ConnectionStartDate.Date;
                var lastDate = connections[i].ConnectionEndDate == null ? DateTime.Now.Date : ((DateTime)connections[i].ConnectionEndDate).Date;
                if (!days.Keys.Contains(lastDate) && connections[i].ConnectionDuration.TotalSeconds > 0)
                {
                    days[lastDate] = new List<ConnectionInfo>();
                }
                if (startDate != lastDate)
                {
                    for(var _date = lastDate.AddDays(-1); _date >= startDate; _date = _date.AddDays(-1))
                    {
                        if (!days.Keys.Contains(_date) && connections[i].ConnectionDuration.TotalSeconds > 0)
                        {
                            days[_date] = new List<ConnectionInfo>();
                        }
                    }
                }
                if (connections[i].ConnectionDuration.TotalSeconds > 0)
                {
                    //if(days.Count() > pageSize * (pageNumber - 1) || pageSize == 0)
                    //{
                    days[lastDate].Add(connections[i]);
                    if (startDate != lastDate)
                    {
                        for (var _date = startDate; _date < lastDate; _date = _date.AddDays(1))
                        {
                            days[_date].Add(connections[i]);
                        }
                    }
                    //}
                }
            }
            if (pageSize != 0)
            {
                var list = days
                    .Select(d => d.Key)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToList();
                var result = new Dictionary<DateTime, List<ConnectionInfo>>();
                foreach (var key in list) result[key] = days[key];
                days = result;
            }
            return days;
        }
        #endregion
    }
}
