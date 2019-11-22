using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using NetVulkanoPruebasAutomatizadas_Front.Models;

namespace NetVulkanoPruebasAutomatizadas_Front.Controllers
{
    public class DetallePruebasEstrategiaController : Controller
    {

        public DetallePruebasEstrategiaController()
        {
           
        }

        public ActionResult Index(int estrategia_id)
        {
            HttpClient client = new HttpClient();
            ViewData["estrategia_id"] = estrategia_id;

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.GetAsync("TipoPrueba/" + estrategia_id).Result;
            List<TipoPrueba> tipoPruebas = new List<TipoPrueba>();
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var mensaje = JsonConvert.DeserializeObject<ReturnMessage>(resultString);   
                tipoPruebas = JsonConvert.DeserializeObject<List<TipoPrueba>>(mensaje.obj.ToString());
            }

            return View(tipoPruebas);
        }

        public ActionResult EnviarPruebaCola(TipoPrueba tipoPrueba,int id_mqTipoPrueba, int estrategia_id)
        {
            HttpClient client = new HttpClient();
            Estrategia estrategia = new Estrategia();
            estrategia.Estrategia_ID = estrategia_id;
            tipoPrueba.MQTipoPrueba.ID = id_mqTipoPrueba;
            estrategia.TipoPruebas.Add(tipoPrueba);

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.PostAsync("RabbitMessages/SendRabbitMessage", estrategia, new JsonMediaTypeFormatter()).Result;
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
           
            }

            return Index(estrategia_id);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="estrategia_id"></param>
        /// <returns></returns>
        public ActionResult EnviarPruebasCola(int estrategia_id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.PostAsync("RabbitMessages/SendRabbitMessages", estrategia_id, new JsonMediaTypeFormatter()).Result;
            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;

            }
            return Index(estrategia_id);
        }
    }
}