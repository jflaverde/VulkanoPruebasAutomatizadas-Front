using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class Estrategia
    {
        public int Estrategia_ID { get; set; }

        [Required]
        [StringLength(20, MinimumLength =5)]
        [Display (Name ="Nombre Estrategia")]
        public string Nombre { get; set; }

        public Estado Estado { get; set; }
        [Required]
        [Display(Name = "Aplicacion")]
        public Aplicacion Aplicacion { get; set; }

        [Required]
        [Display(Name = "Tipos de Prueba")]
        public List<TipoPrueba> TipoPruebas { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 3)]
        [Display(Name = "Version")]
        public string Version { get; set; }
    
        [Display(Name = "Es web")]
        public bool EsWeb { get; set; }
     
        [StringLength(255, MinimumLength = 5)]
        [Display(Name = "URL Aplicacion")]
        public string URLAplicacion { get; set; }
      

        [StringLength(255, MinimumLength = 5)]
        [Display(Name = "Ruta APK")]
        public string RutaAPK { get; set; }

        public Estrategia()
        {
            TipoPruebas = new List<TipoPrueba>();
            Estado = new Estado { ID = 1 };
        }
    }
}
