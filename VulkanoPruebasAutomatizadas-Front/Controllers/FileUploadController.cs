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
                    //var filePath = Path.GetTempFileName();

                    string filename = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    filename = this.EnsureCorrectFilename(filename);

                    var filePath = this._hostingEnvironment.WebRootPath + "\\uploads\\" + filename;
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

        [HttpPost]
        public async Task<IActionResult> upload(IList<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            var filePaths = new List<string>();

            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                filename = this.EnsureCorrectFilename(filename);
                string filePath = this.GetPathAndFilename(filename);

                try
                {
                    using (FileStream output = System.IO.File.Create(filePath))
                    {
                        filePaths.Add(filePath);
                        await source.CopyToAsync(output);
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new { count = files.Count, size, filePaths, message = ex.Message });
                }
                                   
            }

            return Ok(new { count = files.Count, size, filePaths });
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
