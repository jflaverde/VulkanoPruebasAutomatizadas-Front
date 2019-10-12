using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VulkanoPruebasAutomatizadas_Front.Models;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace VulkanoPruebasAutomatizadas_Front.Controllers
{
    public class PruebaEstrategiaController : Controller
    {

        IConfiguration configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PruebaEstrategiaController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Create(TipoPrueba prueba,int estrategia_id)
        {
            ViewData["estrategia_id"] = estrategia_id;
            ViewData["MQTipoPruebas"] = MQTipoPruebasList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(TipoPrueba prueba, int estrategia_id,string rutaScript)
        {
            ReturnMessage response = new ReturnMessage();
            
            if (prueba.Nombre!=null)
            {
                Estrategia estrategia = new Estrategia()
                {
                    Estrategia_ID = estrategia_id
                };

                string baseFileName = Path.GetFileName(rutaScript);
                string ext = Path.GetExtension(rutaScript);

                prueba.Script.Nombre = baseFileName;
                prueba.Script.Extension = ext;

               
                //primero inserto el script
                var message = AddScript(prueba.Script);

                //si el script fue ingresado correctamente paso a insertar la prueba
                if (message.TipoMensaje == TipoMensaje.Correcto)
                {
                    string obj = message.obj.ToString();

                    Script script = JsonConvert.DeserializeObject<Script>(obj);

                    int script_id = script.ID;
                    prueba.Script.ID = script_id;
                    //aqui va la logica de adjunto
                    //TODO:adjuntar y obtener la ruta
                    
                    string path = this._hostingEnvironment.WebRootPath ;
                    string complement = "\\" + script_id.ToString() + "\\";

                    path = path + complement;

                    if (!Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    //Establecer la ruta completa
                    path = path + baseFileName;

                    //Cargar el archivo
                    UploadFile(rutaScript, path);

                    //Ruta relativa, que se guardará en la base de datos
                    prueba.Script.script = complement + baseFileName;
                    prueba.Script.Extension = ext;

                    estrategia.TipoPruebas.Add(prueba);

                    HttpClient cliente = new HttpClient();
                    cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
                    var request = cliente.PostAsync("Estrategia/PostPrueba", estrategia, new JsonMediaTypeFormatter()).Result;

                    if (request.IsSuccessStatusCode)
                    {
                        var resultString = request.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    }
                }                
            }

            ViewData["estrategia_id"] = estrategia_id;
            ViewData["mqTipoPruebas"] = MQTipoPruebasList();
            ViewData["responseMessage"] = response;

            return View();
        }

        /// <summary>
        /// Agrega el script
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public ReturnMessage AddScript(Script script)
        {
            HttpClient cliente = new HttpClient();
            cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
            var request = cliente.PostAsync("Script", script, new JsonMediaTypeFormatter()).Result;

            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                ViewData["responseMessage"] = response;
                return response;
            }
            return new ReturnMessage();
        }

        public List<TipoPrueba> listPruebas(int estrategia)
        {
            return null;
        }

        /// <summary>
        /// Devuelve la lista de los tipos de prueba disponibles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> MQTipoPruebasList()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));

            var request = client.GetAsync("MQTipoPrueba").Result;
            List<MQTipoPrueba> mqTipoPrueba = new List<MQTipoPrueba>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                mqTipoPrueba = JsonConvert.DeserializeObject<List<MQTipoPrueba>>(resultString);
                var selectMqTipoPruebas = mqTipoPrueba
                .Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.Nombre
                }
            );

                return new SelectList(selectMqTipoPruebas, "Value", "Text");
            }

            return null;
        }

        /// <summary>
        /// Cargar el archivo
        /// </summary>
        /// <param name="rutaActual"></param>
        /// <param name="rutaDestino"></param>
        public void UploadFile(string rutaActual,string rutaDestino)
        {
            if (!System.IO.File.Exists(rutaDestino))
            {
                System.IO.File.Copy(rutaActual, rutaDestino, true);
                try
                {
                    System.IO.File.Delete(rutaActual);
                }
                catch (Exception) { }
            }
        }
    }
}