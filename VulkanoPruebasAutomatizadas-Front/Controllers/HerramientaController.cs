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
    public class HerramientaController : Controller
    {
        IConfiguration configuration;

        public HerramientaController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Crear(Herramienta herramienta)
        {
            ViewData["MQTipoPruebas"] = MQTipoPruebasList();
            if (!string.IsNullOrEmpty(herramienta.Nombre))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
                var request = cliente.PostAsync("Herramienta", herramienta, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }
            return View(herramienta);
        }

        public IActionResult Editar(Herramienta herramienta)
        {

            if (!string.IsNullOrEmpty(herramienta.Nombre))
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
                var request = cliente.PutAsync("Herramienta", herramienta, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }
            return View();
        }

        public IActionResult Eliminar(Herramienta herramienta)
        {

            HttpClient cliente = new HttpClient();
            cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
            var request = cliente.DeleteAsync("Herramienta/Eliminar?herramienta_id=" + herramienta.Herramienta_ID).Result;

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

            var request = client.GetAsync("herramienta").Result;
            List<Herramienta> herramientas = new List<Herramienta>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                herramientas = JsonConvert.DeserializeObject<List<Herramienta>>(resultString);
            }
            ViewData["herramientas"] = herramientas;
            return View();
        }

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
    }
}
