using BL;
using Models.Common;
using Models.Users;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Tools;

namespace CorpServer.Controllers
{
    public class UserController : BaseController
    {
        UserBl userBl = new UserBl();
        

        #region Authentication
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginPageVm vm = new LoginPageVm();
            return View(vm);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginPageVm vm)
        {
            try
            {
                //RobotValidate v = new RobotValidate("Admin CP");
                //if (v.ValidateV2(Request.Form["g-recaptcha-response"]))
                //{
                    if (userBl.CheckAdminLogin(vm.AdminModel.Username, vm.AdminModel.Password))
                    {
                        FormsAuthentication.RedirectFromLoginPage(vm.AdminModel.Username.ToLower(), vm.RememberMe);
                        ModelState.Remove("Password");
                    }
                //}
                //else
                //{

                //    ViewBag.ErrorMessage = "Please check i'm not robot!";
                //}

                return View(new LoginPageVm());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
        #endregion

        #region User Custom Functions
        [HttpGet]
        public ActionResult NewUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewUser(User user)
        {
            try
            {
                userBl.CreateUser(user);
                TempData["SuccessMessage"] = string.Format("User {0} Succssusfully Created!", user.UserName);
                return RedirectToAction("UsersLogs", "User");
            }
            catch (TargetInvocationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        
        [HttpGet]
        public ActionResult RemoveUser(string username)
        {
            try
            {
                userBl.DeleteUser(username);
                TempData["SuccessMessage"] = "User Removed Successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            string referringUrl = Request.UrlReferrer?.ToString();
            if (string.IsNullOrEmpty(referringUrl) || !Url.IsLocalUrl(referringUrl))
            {
                return RedirectToAction("UsersLogs", "User");
            }
            return Redirect(referringUrl);
        }
        
        [HttpGet]
        public void DisableUser(string username)
        {
            try
            {
                userBl.DisableUser(username);
                TempData["SuccessMessage"] = "User Locked Successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }
        [HttpGet]
        public void EnableUser(string username)
        {
            try
            {
                userBl.EnableUser(username);
                TempData["SuccessMessage"] = "User Unlocked Successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }

        [HttpGet]
        public ActionResult RemoveUserRedirect(string username)
        {
            try
            {
                userBl.DeleteUser(username);
                TempData["SuccessMessage"] = "User Removed Successfully!";
                return RedirectToAction("UsersLogs", "User");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public ActionResult EditUser(string username)
        {
            try
            {
                var user = userBl.GetUser(username);
                var userVm = new UserProfileVm()
                {
                    SID = user.SID,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Enabled = user.Enabled,
                    ChangePassNextTime = user.ChangePassNextTime,
                    password = "",
                    confirm_password = "",
                    ExtraInfo = user.ExtraInfo,
                    ReservedIpAddress = user.ReservedIpAddress
                };
                return View(userVm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new UserProfileVm());
            }
        }
        [HttpPost]
        public ActionResult EditUser(UserProfileVm user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.password) || !string.IsNullOrEmpty(user.confirm_password))
                {
                    if (user.password != user.confirm_password)
                    {
                        TempData["ErrorMessage"] = "Password and the password confirm must be the same!";
                        return View(user);
                    }
                }
                userBl.UpdateUser(user, change_pass: true, IsAdminAsking: true);
                TempData["SuccessMessage"] = "User Updated Successfully!";
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(user);
            }
        }

        [HttpGet]
        public ActionResult IpReservation()
        {
            try
            {
                return View(userBl.GetIpReservations());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<User>());
            }
        }
        [HttpPost]
        public ActionResult IpReservation(List<User> users)
        {
            try
            {
                userBl.UpdateIpReservations(users);
                TempData["SuccessMessage"] = "Ip Data Updated Successfully!";
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(users);
            }
        }

        #endregion

        #region Event Viewer
        public ActionResult UsersLogs()
        {
            return View();
        }

        public async Task<ActionResult> LoadUsersLogs()
        {
            try
            {
                var startTime = DateTime.Now;
                
                var users = await Task.Run(() => userBl.GetUsersLogs());
                TempData["LoadingTime"] = new TimeSpan((DateTime.Now - startTime).Ticks).ToFriendlyString(shortMode: 1);

                return PartialView("UsersLogs_Partial" , users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message + ex.StackTrace;
                return PartialView("UsersLogs_Partial", new List<UserLogVm>());

            }
        }

        [HttpGet]
        public ActionResult UserLogs(string userName)
        {
            try
            {
                var startTime = DateTime.Now;
                var user = userBl.GetUserLogs(userName);
                TempData["LoadingTime"] = new TimeSpan((DateTime.Now - startTime).Ticks).ToFriendlyString(shortMode: 1);

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new User());
            }
        }
        public void CloseFalseConnection(string username, string connectionId)
        {
            try
            {
                userBl.CloseFalseConnection(username, connectionId);
                TempData["SuccessMessage"] = "Connection closed!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }
        #endregion
    }
}