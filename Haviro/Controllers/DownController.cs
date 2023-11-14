using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Haviro.Controllers
{
    public class DownController : Controller
    {
        public ActionResult Index(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string filePath = "C:\\Web\\Cp\\Uploads\\" + filename; // Adjust the path as needed

                if (System.IO.File.Exists(filePath))
                {
                    // File exists, initiate the download
                    return Redirect($"{Request.Url.Scheme}://{Request.Url.Host}/Cp/Uploads/{filename}");
                }

            }
            return View();
        }
    }
}