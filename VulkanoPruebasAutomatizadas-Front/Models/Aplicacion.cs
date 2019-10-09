namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class Aplicacion
    {
        public int Aplicacion_ID { get; set; }
        public string Nombre { get; set; }
        public string Version { get; set; }
        public string Ruta_Aplicacion { get; set; }
        public bool Es_Web { get; set; }
        public string Descripcion { get; set; }
    }
}