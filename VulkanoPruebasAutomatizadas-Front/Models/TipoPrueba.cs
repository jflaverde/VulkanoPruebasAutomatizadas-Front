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

        public TipoPrueba()
        {
            MQTipoPrueba = new MQTipoPrueba();
            Script = new Script();
        }
    }
}