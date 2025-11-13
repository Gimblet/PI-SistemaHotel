using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Hotel.Controllers;
using System.ComponentModel.DataAnnotations;
namespace Hotel.Models
{
    public class Alquiler
    {
        [Key]
        public int id { get; set; }
        public int idHab { get; set; }
        public int idCli { get; set; }
        public int dias { get; set; }
        public DateTime fechaActual { get; set; }
        public DateTime fechaEntrada { get; set; }
        public DateTime fechaSalida { get; set; }
        public decimal total { get; set; }

    }
}
