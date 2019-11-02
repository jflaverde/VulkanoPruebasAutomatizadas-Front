using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class Herramienta
    {
        public int Herramienta_ID { get; set; }

        [Required]
        [Display(Name = "Nombre Herramienta")] 
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Tipo de Prueba")]
        public string Tipo_Prueba { get; set; }
        [Required]
        [Display(Name = "Es Web?")]
        public bool Es_Web { get; set; }
    }
}
