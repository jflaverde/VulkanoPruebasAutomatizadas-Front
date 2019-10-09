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

namespace VulkanoPruebasAutomatizadas_Front.Controllers
{
    public class EstrategiaController : Controller
    {


        IConfiguration configuration;

        public EstrategiaController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public IActionResult Crear(Estrategia estrategia) {

            ViewData["aplicaciones"] = ApplicationList();
            ViewData["mqTipoPruebas"] = MQTipoPruebasList();
            if (estrategia.Estrategia_ID != 0)
            {
                estrategia.Estado = new Estado()
                {
                    ID = 1
                };

                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
                var request = cliente.PostAsync("Estrategia", estrategia, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }

            return View();
        }

        //pu

        public IActionResult TicketList()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));


            var request = client.GetAsync("estrategia").Result;
            List<Estrategia> estrategias = new List<Estrategia>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                estrategias = JsonConvert.DeserializeObject<List<Estrategia>>(resultString);
            }
            ViewData["estrategias"] = estrategias;
            return View();
        }

        /// <summary>
        /// Devuelve la lista de aplicaciones disponbles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> ApplicationList()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));

            var request = client.GetAsync("aplicacion").Result;
            List<Aplicacion> aplicaciones = new List<Aplicacion>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                aplicaciones = JsonConvert.DeserializeObject<List<Aplicacion>>(resultString);
                var selectAplicaciones = aplicaciones
                .Select(x => new SelectListItem
                {
                    Value = x.Aplicacion_ID.ToString(),
                    Text = x.Nombre
                }
            );

                return new SelectList(selectAplicaciones, "Value", "Text");
            }

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
    }
}
