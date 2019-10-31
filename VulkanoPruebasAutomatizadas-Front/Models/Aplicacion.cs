using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class Aplicacion
    {
        public int Aplicacion_ID { get; set; }

        [Required]
        [Display(Name = "Nombre Aplicación")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Versión de la aplicación")]
        public string Version { get; set; }

        [Display(Name = "Ruta de la aplicación")]
        public string Ruta_Aplicacion { get; set; }
        
        [Display(Name = "Es aplicación Web?")]
        public bool Es_Web { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}