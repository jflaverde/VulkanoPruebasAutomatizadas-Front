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
    public class AplicacionController : Controller
    {
        IConfiguration configuration;

        public AplicacionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Crear(Aplicacion aplicacion){

            if (!string.IsNullOrEmpty(aplicacion.Nombre))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
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

        public IActionResult Actualizar(Aplicacion aplicacion)
        {
            if (!string.IsNullOrEmpty(aplicacion.Nombre))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
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

        public IActionResult Eliminar(Aplicacion aplicacion)
        {
            HttpClient cliente = new HttpClient();
            cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
            var request = cliente.DeleteAsync("Aplicacion/Eliminar?aplicacion_id="+aplicacion.Aplicacion_ID).Result;

            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                ViewData["responseMessage"] = response;
            }

            return Redirect("Lista");
        }

        public IActionResult Lista()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));

            var request = client.GetAsync("aplicacion").Result;
            List<Aplicacion> aplicaciones = new List<Aplicacion>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                aplicaciones = JsonConvert.DeserializeObject<List<Aplicacion>>(resultString);
            }
            ViewData["aplicaciones"] = aplicaciones;
            return View();
        }

        public IActionResult Editar(int aplicacion_id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));

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
    }
}
