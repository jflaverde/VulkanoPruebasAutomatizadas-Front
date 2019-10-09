using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VulkanoPruebasAutomatizadas_Front.Models;

namespace VulkanoPruebasAutomatizadas_Front.Controllers
{
    public class ApplicationController : Controller
    {
        public IActionResult Crear(){

            return View();
        }
    }
}
