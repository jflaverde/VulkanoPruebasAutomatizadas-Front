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
    public class PruebaEstrategiaController : Controller
    {

        IConfiguration configuration;

        public PruebaEstrategiaController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public IActionResult Create(TipoPrueba prueba, int estrategia_id)
        {
            ViewData["mqTipoPruebas"] = MQTipoPruebasList();
            if (prueba.Nombre!=null)
            {
                Estrategia estrategia = new Estrategia()
                {
                    Estrategia_ID = estrategia_id
                };

                estrategia.TipoPruebas.Add(prueba);

                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));
                var request = cliente.PostAsync("Estrategia/postPrueba", estrategia, new JsonMediaTypeFormatter()).Result;

                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<ReturnMessage>(resultString);
                    ViewData["responseMessage"] = response;
                }
            }
            
            return View();
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
    }
}