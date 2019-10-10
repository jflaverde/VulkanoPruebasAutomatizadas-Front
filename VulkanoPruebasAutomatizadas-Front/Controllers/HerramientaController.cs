using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VulkanoPruebasAutomatizadas_Front.Models;
using Newtonsoft.Json;

namespace VulkanoPruebasAutomatizadas_Front.Controllers
{
    public class HerramientaController : Controller
    {


        IConfiguration configuration;

        public HerramientaController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Crear(){
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(configuration.GetValue<string>("Config:APIURL"));


            var request = client.GetAsync("estrategia").Result;

            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                var listado=JsonConvert.DeserializeObject<List<Estrategia>>(resultString);
                Console.WriteLine(listado);
            }
            return View();
        }

        public IActionResult HerramientaList()
        {
            


            return View();
        }
    }
}
