using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VulkanoPruebasAutomatizadas_Front.Models;

namespace VulkanoPruebasAutomatizadas_Front.Controllers
{
    public class DetallePruebasEstrategiaController : Controller
    {

        IConfiguration configuration;

        public DetallePruebasEstrategiaController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index(int estrategia_id)
        {
            HttpClient client = new HttpClient();
            ViewData["estrategia_id"] = estrategia_id;

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));

            var request = client.GetAsync("tipoprueba/" + estrategia_id).Result;
            List<TipoPrueba> tipoPruebas = new List<TipoPrueba>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var mensaje = JsonConvert.DeserializeObject<ReturnMessage>(resultString);   
                tipoPruebas = JsonConvert.DeserializeObject<List<TipoPrueba>>(mensaje.obj.ToString());
            }

            return View(tipoPruebas);
        }

        public IActionResult EnviarPruebaCola(TipoPrueba tipoPrueba,int id_mqTipoPrueba, int estrategia_id)
        {
            HttpClient client = new HttpClient();
            Estrategia estrategia = new Estrategia();
            estrategia.Estrategia_ID = estrategia_id;
            tipoPrueba.MQTipoPrueba.ID = id_mqTipoPrueba;
            estrategia.TipoPruebas.Add(tipoPrueba);

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));

            var request = client.PostAsync("RabbitMessages", estrategia, new JsonMediaTypeFormatter()).Result;
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
           
            }

            return Index(estrategia_id);
           
        }
    }
}