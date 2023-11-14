﻿using BL;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tools;

namespace Client
{
    [Authorize]
    public class BaseController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguages = Request.UserLanguages;

                var userLang = userLanguages != null ? userLanguages[0] : "";

                lang = userLang != "" ? userLang : SiteLanguages.GetDefaultLanguage();
            }

            new SiteLanguages().SetLanguage(lang);

            return base.BeginExecuteCore(callback, state);
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            if(filterContext.Exception is UnauthorizedAccessException)
            {
                FormsAuthentication.SignOut();
                filterContext.Result = RedirectToAction("Login", "Home");
            }
            //Log the error!!
            //_Logger.Error(filterContext.Exception);

            int? statusCode = null;

            if (filterContext.Exception is HttpException)
            {
                statusCode = ((HttpException)filterContext.Exception).GetHttpCode();
            }

            //Redirect or return a view, but not both.
            TempData["ErrorMessage"] = filterContext.Exception.Message;
            if (Request.IsAuthenticated)
            {
                filterContext.Result = RedirectToAction("Index", "Error", new { s = statusCode });
            }
            else
            {
                filterContext.Result = RedirectToAction("LoginError", "Error", new { s = statusCode });
            }
            
            // OR 
            //filterContext.Result = new ViewResult
            //{
            //    ViewName = "~/Areas/Cpanel/Views/Error/Index.cshtml"
            //};
        }
    }
}
