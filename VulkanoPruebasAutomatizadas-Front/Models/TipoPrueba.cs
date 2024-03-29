﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class TipoPrueba
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Nombre Aplicación")]
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
        [StringLength(10, MinimumLength = 1)]
        [Display(Name = "Cantidad Ejecuciones")]
        public int CantidadEjecuciones { get; set; }
        [Required]
        [Display(Name = "Tiempo Ejecucion")]
        public DateTime TiempoEjecucion { get; set; }
        [Required]
        [Display(Name = "Semilla")]
        [StringLength(10, MinimumLength = 1)]
        public string Semilla { get; set; }

        public List<HistorialEjecucionPrueba> HistorialEjecuciones { get; set; }
        public TipoPrueba()
        {
            MQTipoPrueba = new MQTipoPrueba();
            Script = new Script();
            HistorialEjecuciones = new List<HistorialEjecucionPrueba>();
        }
    }
}