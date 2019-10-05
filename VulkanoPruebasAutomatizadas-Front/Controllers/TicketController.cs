using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VulkanoPruebasAutomatizadas_Front.Models;

namespace VulkanoPruebasAutomatizadas_Front.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Crear(){

            
            return View();
        }

        public IActionResult TicketList()
        {
            List<Ticket> tickets = new List<Ticket>();
            for (int i = 0; i < 20; i++)
            {
                var ticket = new Ticket();
                ticket.TicketID = i + 1;
                ticket.TicketName = string.Concat("NombreTicket",ticket.TicketID);
                tickets.Add(ticket);
            }
            ViewData["Tickets"] = tickets;

            return View();
        }
    }
}
