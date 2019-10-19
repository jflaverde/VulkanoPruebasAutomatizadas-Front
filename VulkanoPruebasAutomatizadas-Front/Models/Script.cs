using System.ComponentModel.DataAnnotations;

namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class Script
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Ruta Archivo")]
        public string script { get; set; }
        [Required]
        [Display(Name = "Extensión")]
        public string Extension { get; set; }
    }
}