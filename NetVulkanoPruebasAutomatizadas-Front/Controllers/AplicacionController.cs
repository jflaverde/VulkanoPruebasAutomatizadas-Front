using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NetVulkanoPruebasAutomatizadas_Front.Models;
using System.Net.Http.Formatting;
using System.Configuration;
using System.Web.Mvc;


namespace NetVulkanoPruebasAutomatizadas_Front.Controllers
{
    public class AplicacionController : Controller
    {

        public AplicacionController()
        {
           
        }

        public ActionResult Crear(Aplicacion aplicacion){

            if (!string.IsNullOrEmpty(aplicacion.Nombre))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);
                var request = cliente.PostAsync("Aplicacion", aplicacion, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }
            return View();
        }

        public ActionResult Actualizar(Aplicacion aplicacion)
        {
            if (!string.IsNullOrEmpty(aplicacion.Nombre))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);
                var request = cliente.PutAsync("Aplicacion/Actualizar", aplicacion, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }
            return View();
        }

        public ActionResult Eliminar(Aplicacion aplicacion)
        {
            HttpClient cliente = new HttpClient();
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);
            var request = cliente.DeleteAsync("Aplicacion/Eliminar?aplicacion_id="+aplicacion.Aplicacion_ID).Result;

            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                ViewData["responseMessage"] = response;
            }

            return Redirect("Lista");
        }

        public ActionResult Lista()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.GetAsync("aplicacion").Result;
            List<Aplicacion> aplicaciones = new List<Aplicacion>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var mensaje = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                aplicaciones = JsonConvert.DeserializeObject<List<Aplicacion>>(mensaje.obj.ToString());
            }
            ViewData["aplicaciones"] = aplicaciones;
            return View();
        }

        public ActionResult Editar(int aplicacion_id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.GetAsync("Aplicacion/"+aplicacion_id).Result;
            List<Aplicacion> aplicaciones = new List<Aplicacion>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                aplicaciones = JsonConvert.DeserializeObject<List<Aplicacion>>(resultString);
            }
            ViewData["aplicaciones"] = aplicaciones;
            return View();
        }

        /// <summary>
        /// lista las versiones de la aplicacion
        /// </summary>
        /// <param name="aplicacion_id"></param>
        /// <returns></returns>
        public List<AppVersion> VersionesAplicacion(int aplicacion_id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.GetAsync("appversion/getappversion/" + aplicacion_id).Result;
            List<AppVersion> versiones = new List<AppVersion>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var mensaje = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                versiones = JsonConvert.DeserializeObject<List<AppVersion>>(mensaje.obj.ToString());
                return versiones;
            }
            ViewData["versiones"] = versiones;
            return new List<AppVersion>();
            
        }
    }
}
