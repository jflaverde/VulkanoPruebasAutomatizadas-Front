using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetVulkanoPruebasAutomatizadas_Front.Models
{
    public class TipoPrueba
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Nombre Prueba")]
        public string Nombre { get; set; }
        
        [Display(Name = "Parametros")]
        public string Parametros { get; set; }
        [Required]
        [Display(Name = "Script")]
        public Script Script { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        public Estado Estado { get; set; }
        [Required]
        [Display(Name = "Tipo de prueba")]
        public MQTipoPrueba MQTipoPrueba { get; set; }

        [Required]
        [Display(Name = "Cantidad Ejecuciones")]
        public int CantidadEjecuciones { get; set; }
        [Required]
        [Display(Name = "Tiempo Ejecucion")]
        public double TiempoEjecucion { get; set; }
        [Required]
        [Display(Name = "Semilla")]
        [StringLength(10, MinimumLength = 1)]
        public string Semilla { get; set; }
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "API Controller (Mockaroo)")]
        public string ApiController { get; set; }

        [Display(Name = "API Key (Mockaroo)")]
        [StringLength(9, MinimumLength = 8)]
        public string ApiKey { get; set; }

        public Herramienta Herramienta { get; set; }

        public List<HistorialEjecucionPrueba> HistorialEjecuciones { get; set; }
        public TipoPrueba()
        {
            Herramienta = new Herramienta();
            MQTipoPrueba = new MQTipoPrueba();
            Script = new Script();
            HistorialEjecuciones = new List<HistorialEjecucionPrueba>();
        }
    }
}