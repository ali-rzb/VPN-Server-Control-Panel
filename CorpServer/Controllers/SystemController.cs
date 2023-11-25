using BL;
using Models;
using Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Tools;

namespace CorpServer.Controllers
{
    public class SystemController : BaseController
    {
        SystemBl systemBl = new SystemBl();
        public ActionResult Index() { return View(); }
        [HttpGet]
        public ActionResult Settings(string Message)
        {
            try
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    TempData["ErrorMessage"] = Message;
                }
                ServerSetting setting = systemBl.GetServerSetting();
                return View(setting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Settings(ServerSetting serverSetting)
        {
            try
            {
                bool curent_rule_status = FirewallManagement.CheckRule("RDP TCP").Enabled;
                if (curent_rule_status != serverSetting.RdpRuleStatus)
                {
                    FirewallManagement.ChangeRuleStatus("RDP TCP", serverSetting.RdpRuleStatus);
                    FirewallManagement.ChangeRuleStatus("RDP UDP", serverSetting.RdpRuleStatus);
                }
                serverSetting.RdpRuleStatus = FirewallManagement.CheckRule("RDP TCP").Enabled;
                systemBl.UpdateServerSettings(serverSetting);
                TempData["SuccessMessage"] = "Server Settings Updated Successfully!";
                return View(serverSetting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message + ex.StackTrace;
            }
            return View();
        }


        [HttpGet]
        public JsonResult GetServerPerformance()
        {
            try
            {
                return Json(systemBl.GetServerPerformance(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new SystemPerformance());
            }
        }

        [HttpGet]
        public ActionResult ConnectionsPerDay()
        {
            try
            {
                var timer = DateTime.Now;
                var results = systemBl.CountConnectionsPerDay(120, 2);
                TempData["LoadingTime"] = (DateTime.Now - timer).ToFriendlyString(shortMode: 1);
                return View(results);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message + ex.StackTrace;
                return View();
            }
        }


        public void Delete(string fileName)
        {
            var path = Server.MapPath("~/Uploads/" + fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            Response.Redirect("~/System/Upload");
        }

        private List<FileInfo> GetFiles()
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            var directory = new DirectoryInfo(Server.MapPath("~/Uploads"));
            var files = directory.GetFiles();
            return files.ToList();
        }
        [HttpGet]
        public ActionResult Upload()
        {
            return View(GetFiles());
        }

        public ActionResult FilesPartial()
        {
            var files = GetFiles();
            return PartialView("ListFilesPartial", files);
        }
        private string CheckFileName(string defultName)
        {
            var fileName = Path.GetFileName(defultName);
            var extension = Path.GetExtension(fileName);
            var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);

            // Replace invalid characters in file name with underscore
            var safeFileName = string.Concat(fileNameOnly.Split(Path.GetInvalidFileNameChars())).Trim();

            var i = 0;
            var path = Path.Combine(Server.MapPath("~/Uploads"), string.Format("{0}{1}", safeFileName, extension));
            while (System.IO.File.Exists(path))
            {
                i++;
                fileName = string.Format("{0} ({1}){2}", safeFileName, i, extension);
                path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
            }
            return path;

        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(CheckFileName(file.FileName));
                    return View(GetFiles());
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(ex.Message);
            }
        }
        private static long GetFileSize(string url)
        {
            long fileSize = 0;
            WebRequest request = WebRequest.Create(url);
            request.Method = "HEAD";
            using (WebResponse response = request.GetResponse())
            {
                if (long.TryParse(response.Headers.Get("Content-Length"), out long contentLength))
                {
                    fileSize = contentLength;
                }
            }
            return fileSize;
        }
        public ActionResult UploadFromLink(string url)
        {
            try
            {
                var fileName = Path.GetFileName(url);
                fileName = CheckFileName(fileName);
                var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);

                
                long filesize = GetFileSize(url);

                // Write the download information to a text file
                string downloadInfo = string.Format("{0} &&& {1} &&& {2} &&& {3}", 
                    fileName, filesize, DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"), url);

                using (StreamWriter writer = new StreamWriter(Server.MapPath("~/UrlUploadInfo.txt"), true))
                {
                    writer.WriteLine(downloadInfo);
                }


                WebClient client = new WebClient();
                client.DownloadFile(url, path);
                // Return a success message as a JSON response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Upload");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult FileViewer(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                var decrypred_fileName = Encryption.DeobfuscateFileName(fileName);
                string filePath = Server.MapPath("~/Uploads/" + decrypred_fileName);
                var file = new FileInfo(filePath);
                return View("FileViewer", model: file);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        public JsonResult RemoveFileFromList(string fileName)
        {
            string filePath = Server.MapPath("~/UrlUploadInfo.txt");
            string tempFilePath = Path.GetTempFileName();

            StreamReader reader = null;
            StreamWriter writer = null;

            try
            {
                reader = new StreamReader(filePath);
                writer = new StreamWriter(tempFilePath);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries)[0].EndsWith("\\" + fileName))
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (writer != null) writer.Close();
            }

            System.IO.File.Delete(filePath);
            System.IO.File.Move(tempFilePath, filePath);

            return Json(new { success = true });
        }
        public ActionResult GetUploadedFilesJson()
        {
            string url_upload_path = Server.MapPath("~/UrlUploadInfo.txt");
            if (System.IO.File.Exists(url_upload_path))
            {
                var fileSizes = new Dictionary<string, Tuple<long, long, string, string>>();
                foreach (var line in System.IO.File.ReadLines(url_upload_path))
                {
                    var parts = line.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries);
                    var filePath = parts[0];
                    var fileName = Path.GetFileName(filePath);
                    long fileSize;
                    long downloaded = 0;
                    if (long.TryParse(parts[1].Trim(), out fileSize))
                    {
                        var fileInfo = new FileInfo(filePath);
                        if (fileInfo.Exists)
                            downloaded = fileInfo.Length;
                        else
                            RemoveFileFromList(fileName);
                        if(downloaded >= fileSize)
                            RemoveFileFromList(fileName);
                        fileSizes[filePath] = Tuple.Create(downloaded, fileSize, fileName, InfoTransferUnit.ParseBytes(fileSize).ToString());
                    }
                }
                var json = JsonConvert.SerializeObject(fileSizes);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null);
            }

        }
    }
}