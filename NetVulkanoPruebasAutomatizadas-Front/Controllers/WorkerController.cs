using NetVulkanoPruebasAutomatizadas_Front.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace NetVulkanoPruebasAutomatizadas_Front.Controllers
{
    public class WorkerController : Controller
    {
     
        /// <summary>
        /// Listado de workers
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkerStatusList()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIURL"]);

            var request = client.GetAsync("Worker").Result;
            List<WorkerStatus> workerList = new List<WorkerStatus>();

            if (request.IsSuccessStatusCode)
            {
                var resultString = request.Content.ReadAsStringAsync().Result;
                workerList = JsonConvert.DeserializeObject<List<WorkerStatus>>(resultString);
                ViewData["workers"] = workerList;
                    }
            return View(workerList);
        }
    }
}