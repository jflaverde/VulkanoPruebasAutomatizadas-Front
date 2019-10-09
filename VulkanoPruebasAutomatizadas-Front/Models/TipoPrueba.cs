using System.Collections.Generic;

namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class TipoPrueba
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Parametros { get; set; }
        public List<Script> Scripts { get; set; }
        public string Descripcion { get; set; }
        public Estado Estado { get; set; }
        public MQTipoPrueba MQTipoPrueba { get; set; }
    }
}