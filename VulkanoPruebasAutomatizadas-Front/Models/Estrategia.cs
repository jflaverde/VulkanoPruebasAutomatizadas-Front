using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VulkanoPruebasAutomatizadas_Front.Models
{
    public class Estrategia
    {

    public int Estrategia_ID { get; set; }
    public string Nombre { get; set; }
    public Estado Estado { get; set; }
    public Aplicacion Aplicacion { get; set; }
    public List<TipoPrueba> TipoPruebas { get; set; }
    }
}
