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
        [Display(Name = "Version")]
        public string Version { get; set; }
        [Required]
        [Display(Name = "Es web")]
        public bool EsWeb { get; set; }
        [Required]
        [Display(Name = "URL Aplicacion")]
        public string URLAplicacion { get; set; }
        [Required]
        [Display(Name = "Ruta APK")]
        public string RutaAPK { get; set; }

        public Estrategia()
        {
            TipoPruebas = new List<TipoPrueba>();
        }
    }
}
