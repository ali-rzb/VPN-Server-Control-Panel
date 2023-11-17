using BL;
using Models.Users;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tools;

namespace Client.Controllers
{
    public class HomeController : BaseController
    {
        UserBl userBl = new UserBl();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginPageVm vm = new LoginPageVm();
            return View(vm);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginPageVm vm)
        {
            try
            {
                RobotValidate v = new RobotValidate("Client CP");
                if (Task.Run(() => v.ValidateV3(vm.GoogleRecaptchaToken, Request.UserHostAddress, "Login")).Result)
                {
                    var validate = userBl.CheckClientLogin(vm.AdminModel.Username.ToLower(), vm.AdminModel.Password);
                    if (validate == true)
                    {
                        ModelState.Remove("Password");
                        FormsAuthentication.RedirectFromLoginPage(vm.AdminModel.Username.ToLower(), vm.RememberMe);
                    }
                    // user locked
                    else if (validate == null)
                    {
                        return await Task.Run<ActionResult>(() =>
                        {
                            return RedirectToAction("ChangePass", "Home", new { username = vm.AdminModel.Username });
                        });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Bad Username or password!";
                        return await Task.FromResult(View(new LoginPageVm()));
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "You have suspicious activities! try again later.";
                    return await Task.FromResult(View(new LoginPageVm()));
                }
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("The user's password must be changed before signing in."))
                {
                    return await Task.Run<ActionResult>(() =>
                    {
                        return RedirectToAction("ChangePass", "Home", new { username = vm.AdminModel.Username });
                    });
                }
                TempData["ErrorMessage"] = ex.Message;
            }

            return await Task.FromResult(View());
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangePass(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                ChangePassVm vm = new ChangePassVm()
                {
                    Username = username,
                };
                return View(vm);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePass(ChangePassVm vm)
        {
            try
            {
                RobotValidate v = new RobotValidate("Client CP");
                if (Task.Run(() => v.ValidateV3(vm.GoogleRecaptchaToken, Request.UserHostAddress, "Login")).Result)
                {
                    if (vm.NewPassword != vm.ConfirmPassword)
                    {
                        TempData["ErrorMessage"] = "Password and confirm password must to be the same!";
                        return View(vm);
                    }
                    else
                    {
                        bool? validate = false;
                        try
                        {
                            validate = userBl.CheckClientLogin(vm.Username.ToLower(), vm.CurrentPassword);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("The user's password must be changed before signing in."))
                            {
                                validate = null;
                            }
                        }
                        if (validate == true || validate == null)
                        {
                            try
                            {
                                userBl.ChangePasswordAndUnlock(vm);
                                TempData["SuccessMessage"] = "Password Changed Successfully!";
                                return await Task.FromResult(RedirectToAction("Login"));
                            }
                            catch (Exception ex)
                            {
                                TempData["ErrorMessage"] = ex.Message;
                                return View(vm);
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Bad Username or Password!";
                            return View(vm);
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "You have suspicious activities! try again later.";
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                if(ex.Message == "User Not Found!")
                {
                    TempData["ErrorMessage"] = "Bad Username or Password!";
                    return View(vm);
                }
                else
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View(vm);
                }
            }
        }


        public ActionResult Index()
        {
            try
            {
                var userName = System.Web.HttpContext.Current.User.Identity.Name;
                var startTime = DateTime.Now;
                var user = userBl.GetUserLogs(userName);
                TempData["LoadingTime"] = new TimeSpan((DateTime.Now - startTime).Ticks).ToFriendlyString(shortMode: 1);
                return View(user);
            }
            catch (Exception ex)
            {
                if(ex.Message == "Account not found!")
                {
                    throw new UnauthorizedAccessException();
                }
                TempData["ErrorMessage"] = ex.Message;
                return View(new User());
            }
        }
      
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
        public ActionResult UserProfile()
        {
            try
            {
                var userName = System.Web.HttpContext.Current.User.Identity.Name;
                var user = userBl.GetUser(userName);
                var userVm = new UserProfileVm()
                {
                    SID = user.SID,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    password = "",
                    confirm_password = "",
                    Enabled = user.Enabled,
                    ChangePassNextTime = user.ChangePassNextTime,
                    ExtraInfo = user.ExtraInfo,
                    ReservedIpAddress = user.ReservedIpAddress
                };
                return View(userVm);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Account not found!")
                {
                    throw new UnauthorizedAccessException();
                }
                TempData["ErrorMessage"] = ex.Message;
                return View(new User());
            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserProfile(UserProfileVm user)
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
                SystemBl systemBl = new SystemBl();
                bool change_pass_access = systemBl.GetServerSetting().ClientsCanChangePass;
                //bool change_pass_access = true;
                userBl.UpdateUser(user, change_pass: change_pass_access, IsAdminAsking:false);
                TempData["SuccessMessage"] = "User Updated Successfully!";
                return View(user);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Account not found!")
                {
                    throw new UnauthorizedAccessException();
                }
                TempData["ErrorMessage"] = ex.Message;
                return View(new UserProfileVm());
            }
        }
    }
}