using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Hotel.Controllers;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Models
{
    public class Habitacion
    {
        public int Id { get; set; }
        public string? Numero { get; set; }
        public int TipoId { get; set; }
        public int EstadoId { get; set; }
    }
}
