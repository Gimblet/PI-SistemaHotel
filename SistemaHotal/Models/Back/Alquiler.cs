using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using SistemaHotal.Controllers;
using System.ComponentModel.DataAnnotations;
namespace SistemaHotal.Models.Back
{
    public class Alquiler
    {
        public int id { get; set; }
        public Habitacion? idHab { get; set; }
        public Cliente? idCli { get; set; }
        public int dias { get; set; }
        public DateTime fechaActual { get; set; }
        public DateTime fechaEntrada { get; set; }
        public DateTime fechaSalida { get; set; }
    }
}
