using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetVulkanoPruebasAutomatizadas_Front.Models
{
    public class WorkerStatus
    {
        public int WorkerID { get; set; }
        public string TipoPrueba { get; set; }
        public Estado Estado { get; set; }
    }
}