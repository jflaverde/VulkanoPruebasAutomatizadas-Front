using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

namespace IdeeaCMS.Controllers
{
    /// <summary>
    /// Controlador que implementa la lógica para cargar archivos multiples
    /// </summary>
    public class FileUploadController : Controller
    {

        private string SessionKey = null;

        /// <summary>
        /// Vista de prueba para cargar archivos en una ubicación específica
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// Muestra el formulario para cargar archivos usando el componente.
        /// </summary>
        /// <param name="Path">Recibe la ruta en la cual se cargarán los archivos temporalmente. 
        /// La ruta se toma a partir de la carpeta de carga de archivos por defecto configurad</param>
        /// <param name="SessionKey">Nombre de la llave que registrara los archivos en sesion antes de ser guardados</param>
        /// <param name="allowedExtensions">Extensiones permitidas para cargar en el siguiente formato jpg|jpeg|gif|png. 
        /// Si no se define un valor, se usan las siguientes extensiones por defecto jpg|jpeg|gif|png</param>
        /// <returns></returns>
        public ActionResult Upload(string Path, string SessionKey, string allowedExtensions)
        {
            ViewData["Path"] = Path;
            ViewData["SessionKey"] = SessionKey;
            this.SessionKey = SessionKey;

            if (!string.IsNullOrEmpty(allowedExtensions))
            {
                ViewData["AllowedExtensions"] = allowedExtensions;
            }
            else
            {
                ViewData["AllowedExtensions"] = "jpg|jpeg|gif|png";
            }

            return View("Upload");
        }

        /// <summary>
        /// Carga cada uno de los archivos en la carpeta temporal y registra sus nombres en la sesión
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload()
        {
            IList<string> savedFiles = (IList<string>)Session[Request["SessionKey"]];
            this.SessionKey = Convert.ToString(Request["SessionKey"]);

            if (savedFiles == null)
            {
                savedFiles = new List<string>();
            }
            string basePath = ConfigurationManager.AppSettings["BaseFilePath"] + "/" + Request["Path"];
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
                               
                    //ViewDataUploadFilesResult fileResult = UploadResult(basePath, file, fullName);


                    string ruta = basePath + "/" + fullName;
                    savedFiles.Add(ruta);

                    Session[Request["SessionKey"]] = savedFiles;

                    if (savedFiles == null)
                    {
                        savedFiles = new List<string>();
                    }

                    List<ViewDataUploadFilesResult> fileList = new List<ViewDataUploadFilesResult>();
                    //fileList.Add(fileResult);
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
        }

        /// <summary>
        /// Obtiene los archivos cargados en el sistema para mostraros en un componente
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult GetUploadedFiles()
        {
            IList<string> savedFiles = (IList<string>)Session[Request["SessionKey"]];
            if (savedFiles == null)
            {
                savedFiles = new List<string>();
            }

            IList<ViewDataUploadFilesResult> files = LoadFiles(savedFiles);

            return View(files);
        }

        /// <summary>
        /// Permite cargar la lista de los archivos a partir de la lista de los archivos recibidos
        /// </summary>
        /// <param name="savedFileNames">Lista de rutas de los archivos a cargar</param>
        /// <returns></returns>
        private IList<ViewDataUploadFilesResult> LoadFiles(IList<string> savedFileNames)
        {
            IList<ViewDataUploadFilesResult> files = new List<ViewDataUploadFilesResult>();
            foreach (string virtualPath in savedFileNames)
            {
                ViewDataUploadFilesResult uploadedFile = UploadResult(virtualPath);
                if (uploadedFile != null)
                {
                    files.Add(uploadedFile);
                }
            }
            return files;
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        /// <summary>
        /// Permite eliminar un archivo previamente cargado.
        /// </summary>
        /// <param name="file">Rura completa del archivo a eliminar a partir de la base de tos archivos (~/Files)</param>
        /// <param name="storageKey">LLave de la sesión en la cual se almacenan temporalmente los nombres de los archivos.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete(string file, string storageKey)
        {
            var filename = file;
            var filePath = Path.Combine(Server.MapPath(filename), "");

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                //Se elimina también de la sesión
                IList<string> savedFiles = (IList<string>)Session[storageKey];

                if (savedFiles != null && savedFiles.Count > 0)
                {
                    //Int32 index = savedFiles.IndexOf(file);

                    foreach (string fileString in savedFiles)
                    {

                        if (fileString.Equals(file))
                        {
                            savedFiles.Remove(file);
                            break;
                        }
                    }

                    Session[storageKey] = savedFiles;
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        /// <summary>
        /// Permite descargar uno de los archivos cargados previamente. Se debe recibir como parámetro la rutas completa del archivo a descargar a partir de la base de los archivos.
        /// </summary>
        /// <param name="file"></param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void Download(string file)
        {
            var filename = file;
            var filePath = Path.Combine(Server.MapPath(filename), "");

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        /// <summary>
        /// Genera el resultado del archivo cargado.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private ViewDataUploadFilesResult UploadResult(string path, HttpPostedFileBase file, string fullName)
        {
            string thumbUrl = GenerateThumbnailUrl(path + "/" + fullName);
            ViewDataUploadFilesResult fileResult = new ViewDataUploadFilesResult()
            {
                name = file.FileName,
                size = file.ContentLength,
                type = file.ContentType,
                url = "/Upload/Download/?file=" + path + "/" + file.FileName,
                deleteUrl = "/Upload/Delete/?storageKey=" + SessionKey + "&file=" + path + "/" + file.FileName,
                thumbnailUrl = @"data:image/png;base64," + EncodeFile(thumbUrl),
                deleteType = "GET",
                VirtualPath = path + "/" + file.FileName,
            };

            return fileResult;
        }

        /// <summary>
        /// Permite cargar las condiciones del archivo.
        /// </summary>
        /// <param name="virtualPath">Ruta virtual completa del archivo.</param>
        /// <returns></returns>
        private ViewDataUploadFilesResult UploadResult(string virtualPath)
        {
            string SessionKeyName = Request["SessionKey"];
            string fullPath = Server.MapPath(virtualPath);
            string thumbUrl = GenerateThumbnailUrl(virtualPath);
            if (System.IO.File.Exists(fullPath))
            {
                FileInfo info = new FileInfo(fullPath);

                ViewDataUploadFilesResult fileResult = new ViewDataUploadFilesResult()
                {
                    name = info.Name,
                    size = (int)info.Length,
                    type = "",
                    url = "/Upload/Download/?file=" + virtualPath,
                    deleteUrl = "/Upload/Delete/?storageKey=" + SessionKeyName + "&file=" + virtualPath,
                    thumbnailUrl = @"data:image/png;base64," + EncodeFile(thumbUrl),
                    deleteType = "GET",
                    VirtualPath = virtualPath,
                };

                return fileResult;
            }

            return null;
        }

        /// <summary>
        /// Genera el Preview de la imagen
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        private string GenerateThumbnailUrl(string virtualPath)
        {
            string fullPath = Server.MapPath(virtualPath);
            string thumbUrl = "";

            FileStream fileStream = null;
            //Si es una imágen se genera el preview, de lo contrario se genera el previo por defecto.
            if (IsImageFromExt(fullPath))
            {
                try
                {
                    fileStream = new FileStream(Path.Combine(Server.MapPath(virtualPath)), FileMode.OpenOrCreate);
                    Image image = Image.FromStream(fileStream);
                    string fileName = virtualPath.Split('/').Last();
                    string filePath = virtualPath.Remove(virtualPath.LastIndexOf('/'));
                    thumbUrl = filePath + "/" + "80x80_" + fileName;
                    FileStream fileThunm = new FileStream(Server.MapPath(thumbUrl), FileMode.OpenOrCreate);
                    Image origin = new Bitmap(image, new Size(80, 80));
                    origin.Save(fileThunm, ImageFormat.Jpeg);
                    fileThunm.Close();
                    fileStream.Close();
                    thumbUrl = Server.MapPath(thumbUrl);
                }
                catch (Exception ex)
                {
                    thumbUrl = Server.MapPath("~/Images/uploadIcons/alert.png");
                    fileStream.Close();
                }

            }
            else
            {
                string ext = Path.GetExtension(fullPath);
                if (ext.ToLower() == ".doc" || ext.ToLower() == ".docx")
                {
                    thumbUrl = Server.MapPath("~/Images/uploadIcons/doc.png");
                }
                else if (ext.ToLower() == ".pdf")
                {
                    thumbUrl = Server.MapPath("~/Images/uploadIcons/pdf.jpg");
                }
                else
                {
                    thumbUrl = Server.MapPath("~/Images/uploadIcons/generalFile.png"); //TODO Poner previw segun la extensión
                }
            }
            return thumbUrl;
        }

        /// <summary>
        /// Permite identificar si es una imágen a partir del Content Type del archivo
        /// </summary>
        /// <param name="contentType">ContentType del archivo</param>
        /// <returns></returns>
        private bool IsImage(string contentType)
        {
            string cType = contentType.ToLower();
            return (cType == "image/jpg" || cType == "image/jpeg" || cType == "image/png" || cType == "image/gif");
        }

        /// <summary>
        /// Permite identificar si es una imágen a partir de la extensión del archivo.
        /// </summary>
        /// <param name="filePath">Rura del archivo incluido el nombre del archivo</param>
        /// <returns></returns>
        private bool IsImageFromExt(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            return (!string.IsNullOrEmpty(ext) && (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".gif" || ext.ToLower() == ".png"));
        }
    }

    [Serializable]
    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }
        public string VirtualPath { get; set; }

        public string SizeFormated
        {
            get
            {
                if (size >= 1000000000)
                {
                    return Math.Round(((double)(size / 1000000000)), 2).ToString() + " GB";
                }
                if (size >= 1000000)
                {
                    return Math.Round(((double)(size / 1000000)), 2).ToString() + " MB";
                }
                return Math.Round(((double)(size / 1000)), 2).ToString() + " KB";
            }
        }
    }

    public class JsonFiles
    {
        public ViewDataUploadFilesResult[] files;
        public string TempFolder { get; set; }
        public JsonFiles(List<ViewDataUploadFilesResult> filesList)
        {
            files = new ViewDataUploadFilesResult[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }

        }
    }
}