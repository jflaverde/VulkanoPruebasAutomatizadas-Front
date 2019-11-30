using NetVulkanoPruebasAutomatizadas_Front.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;

namespace NetVulkanoPruebasAutomatizadas_Front.Controllers
{
    public class AppVersionController : Controller
    {
        // GET: AppVersion
        public ActionResult Create(AppVersion appVersion)
        {
            ViewData["aplicaciones"] = ApplicationList();

            if(!string.IsNullOrEmpty(appVersion.Numero))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);
                var request = cliente.PostAsync("AppVersion", appVersion, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }

            return View(appVersion);
        }

        /// <summary>
        /// Devuelve la lista de aplicaciones disponbles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> ApplicationList()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.GetAsync("aplicacion").Result;
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var mensaje = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                var aplicaciones = JsonConvert.DeserializeObject<List<Aplicacion>>(mensaje.obj.ToString());
                var selectAplicaciones = aplicaciones
                .Select(x => new SelectListItem
                {
                    Value = x.Aplicacion_ID.ToString(),
                    Text = x.Nombre
                }
            );

                ViewData["objAplicaciones"] = aplicaciones;
                return new SelectList(selectAplicaciones, "Value", "Text");
            }

            return null;
        }
    }
}