using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class AppVersion
    {
        public int AppVersion_ID { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 5)]
        [Display(Name = "Número de la Version")]
        public string Numero { get; set; }
    }
}