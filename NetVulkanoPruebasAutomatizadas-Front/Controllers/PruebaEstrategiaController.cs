using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NetVulkanoPruebasAutomatizadas_Front.Models;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Web.Mvc;
using System.Configuration;
using System.Web;

namespace NetVulkanoPruebasAutomatizadas_Front.Controllers
{
    public class PruebaEstrategiaController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public PruebaEstrategiaController()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prueba"></param>
        /// <param name="estrategia_id"></param>
        /// <param name="es_web"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public ActionResult Create(TipoPrueba prueba,int estrategia_id,bool es_web, HttpPostedFileBase file)
        {
            ViewData["estrategia_id"] = estrategia_id;
            ViewData["MQTipoPruebas"] = MQTipoPruebasList();
            ViewData["Herramientas"] = GetHerramientas(es_web);
            ViewData["es_web"] = es_web;
            return View();
        }

        /// <summary>
        /// Obtiene las herramientas asociadas al tipo de prueba web o movil
        /// </summary>
        /// <param name="es_web"></param>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetHerramientas(bool es_web)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.GetAsync("Herramienta").Result;
            List<Herramienta> herramientas = new List<Herramienta>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                herramientas = JsonConvert.DeserializeObject<List<Herramienta>>(resultString);
                var herramientasFiltradas = herramientas.FindAll(h=>h.Es_Web==es_web)
                .Select(x => new SelectListItem
                {
                    Value = x.Herramienta_ID.ToString(),
                    Text = x.Nombre
                }
            );
                return new SelectList(herramientasFiltradas, "Value", "Text");
            }

            return null;
        }

        [HttpPost]
        public ActionResult Create(TipoPrueba prueba, int estrategia_id,bool es_web)
        {
            ReturnMessage response = new ReturnMessage();

            prueba.Semilla = string.IsNullOrEmpty(prueba.Semilla) ? string.Empty : prueba.Semilla;


            if(prueba.TiempoEjecucion!=0)
            {
                double segundos=prueba.TiempoEjecucion/1000;
                double minutos=segundos/60;
                double horas=minutos/60;
                prueba.TiempoEjecucion = horas;
            }

            if (prueba.Nombre!=null)
            {
                Estrategia estrategia = new Estrategia()
                {
                    Estrategia_ID = estrategia_id
                };

                IList<string> savedFiles = (IList<string>)Session["Attach_" + estrategia_id];
                string rutaScript = savedFiles[0];

                string baseFileName = Path.GetFileNameWithoutExtension(rutaScript);
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

                    string path = ConfigurationManager.AppSettings["RutaScript"];

                    estrategia.TipoPruebas.Add(prueba);

                    HttpClient cliente = new HttpClient();
                    cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);
                    var request = cliente.PostAsync("TipoPrueba", estrategia, new JsonMediaTypeFormatter()).Result;

                    if (request.IsSuccessStatusCode)
                    {
                        var resultString = request.Content.ReadAsStringAsync().Result;
                        var response1 = JsonConvert.DeserializeObject<ReturnMessage>(resultString);

                        string objtTipoPrueba = response1.obj.ToString();

                        TipoPrueba tipoPrueba = JsonConvert.DeserializeObject<TipoPrueba>(objtTipoPrueba);

                        #region Actualizar la ruta del script

                        string complement = Path.Combine(tipoPrueba.ID.ToString(), script_id.ToString());

                        complement = string.Concat(complement, "\\");

                        //string.Concat("\\",tipoPrueba.ID,"\\",script_id.ToString(),"\\");

                        path = path + complement;

                        if (!Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        //Establecer la ruta completa
                        path = path + baseFileName;

                        //Cargar el archivo
                        UploadFile(rutaScript, string.Concat(path,ext));

                        //Ruta relativa, que se guardará en la base de datos
                        prueba.Script.script = complement;
                        prueba.Script.Extension = ext;

                        ReturnMessage returnMessage = PutScript(prueba.Script);
                        #endregion
                    }

                }                
            }

            ViewData["estrategia_id"] = estrategia_id;
            ViewData["mqTipoPruebas"] = MQTipoPruebasList();
            ViewData["responseMessage"] = response;
            ViewData["Herramientas"] = GetHerramientas(es_web);
            ViewData["es_web"] = es_web;

            return View();
        }

        /// <summary>
        /// actualiza la ruta del script en la base de datos
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public ReturnMessage PutScript(Script script)
        {
            HttpClient cliente = new HttpClient();
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);
            var request = cliente.PutAsync("Script", script, new JsonMediaTypeFormatter()).Result;

            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                return response;
            }
            return new ReturnMessage();
           
        }

        /// <summary>
        /// Agrega el script
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public ReturnMessage AddScript(Script script)
        {
            HttpClient cliente = new HttpClient();
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="estrategia"></param>
        /// <returns></returns>
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

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

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
            rutaActual = Server.MapPath(rutaActual);
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