using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetVulkanoPruebasAutomatizadas_Front.Models
{
    public class ReturnMessage
    {
        public TipoMensaje TipoMensaje { get; set; }

        public string Mensaje { get; set; }

        public object obj { get; set; }
     
    }
}

public enum TipoMensaje
{
    Correcto = 1,
    Error = 2
}