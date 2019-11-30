using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetVulkanoPruebasAutomatizadas_Front.Models
{
    public class Aplicacion
    {
        public int Aplicacion_ID { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 5)]
        [Display(Name = "Nombre Aplicación")]
        public string Nombre { get; set; }
        
        [Display(Name = "Es aplicación Web?")]
        public bool Es_Web { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(50, MinimumLength = 5)]
        public string Descripcion { get; set; }
    }
}