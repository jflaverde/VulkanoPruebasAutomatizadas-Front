using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace VulkanoPruebasAutomatizadas_Front.Controllers
{   
    public class FileUploadController : Controller
    {

        private readonly IHostingEnvironment _hostingEnvironment;

        public FileUploadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }



        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    var filePath = Path.GetTempFileName();
                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePaths });
        }


        // [AcceptVerbs(HttpVerbs.Post)]
        /*public JsonResult Upload()
        {
            IList<string> savedFiles = (IList<string>)Session[Request["SessionKey"]];
            this.SessionKey = Convert.ToString(Request["SessionKey"]);

            if (savedFiles == null)
            {
                savedFiles = new List<string>();
            }

            string basePath = ConfigurationSettings.AppSettings["BaseFilePath"] + "/" + Request["Path"];
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = (HttpPostedFileBase)Request.Files[0];

                    var fullName = Path.GetFileName(file.FileName.Length > 30 ? file.FileName.Remove(30) : file.FileName);
                    fullName = Regex.Replace(fullName, "[^0-9a-zA-Z.-_~/ ]", "");
                    fullName = fullName.Replace(" ", "");
                    if (!Path.HasExtension(fullName))
                        fullName += Path.GetExtension(file.FileName);


                    string oldFullName = fullName;

                    if (IsImage(file.ContentType))
                    {

                        Image image = new Bitmap(file.InputStream);

                        if (file.ContentLength > 524000)
                        {
                            double width = image.Width;
                            double height = image.Height;
                            do
                            {

                                width /= 1.12;
                                height /= 1.12;
                            }
                            while (width > 2000);

                            FileStream fileStream = new FileStream(Path.Combine(Server.MapPath(basePath), fullName), FileMode.OpenOrCreate);
                            Image origin = new Bitmap(image, new Size(Convert.ToInt32(width), Convert.ToInt32(height)));
                            origin.Save(fileStream, ImageFormat.Jpeg);
                            fileStream.Close();

                        }
                        else
                        {

                            file.SaveAs(Path.Combine(Server.MapPath(basePath), fullName));
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath(basePath), oldFullName);
                        file.SaveAs(path);
                    }



                    ViewDataUploadFilesResult fileResult = UploadResult(basePath, file, fullName);


                    string ruta = basePath + "/" + fullName;
                    savedFiles.Add(ruta);

                    Session[Request["SessionKey"]] = savedFiles;

                    if (savedFiles == null)
                    {
                        savedFiles = new List<string>();
                    }

                    List<ViewDataUploadFilesResult> fileList = new List<ViewDataUploadFilesResult>();
                    fileList.Add(fileResult);
                    JsonFiles files = new JsonFiles(fileList);

                    return Json(files);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }*/


        [HttpPost]
        public async Task<IActionResult> upload(IList<IFormFile> files)
        {
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                filename = this.EnsureCorrectFilename(filename);


                try
                {
                    using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename)))
                    {
                        await source.CopyToAsync(output);
                    }

                }
                catch (Exception ex)
                {

                }
                                   
            }

            return this.View();
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            
            return this._hostingEnvironment.WebRootPath + "\\uploads\\" + filename;
        }
    }
}
