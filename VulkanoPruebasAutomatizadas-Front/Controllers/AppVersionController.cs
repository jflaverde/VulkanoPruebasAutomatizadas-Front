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

namespace VulkanoPruebasAutomatizadas_Front.Controllers
{
    public class AppVersionController : Controller
    {
        IConfiguration configuration;

        public AppVersionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Crear(AppVersion appversion){

            if (!string.IsNullOrEmpty(appversion.Numero))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
                var request = cliente.PostAsync("AppVersion", appversion, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }
            return View();
        }


        public IActionResult Lista()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));

            var request = client.GetAsync("aplicacion").Result;
            List<AppVersion> appversiones = new List<AppVersion>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var mensaje = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                appversiones = JsonConvert.DeserializeObject<List<AppVersion>>(mensaje.obj.ToString());
            }
            ViewData["appversiones"] = appversiones;
            return View();
        }
    }
}
