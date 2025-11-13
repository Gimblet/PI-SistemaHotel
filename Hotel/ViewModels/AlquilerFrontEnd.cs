using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.ViewModels
{
    public class AlquilerFrontEnd
    {
        public int id { get; set; }
        public int HabitacionId { get; set; }
        public string? Numero { get; set; }
        public string? TipoHabitacion { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int estado { get; set; }

        // Datos del cliente
        public int idCli { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int tipoDocId { get; set; }
        public string? tipoDoc { get; set; }
        //public List<SelectListItem> tipoDoc { get; set; } = new List<SelectListItem>();
        public string? Documento { get; set; }
        public string? Telefono { get; set; }

        // Datos del alquiler
        public int dias { get; set; }
        public DateTime FechaActual { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal total { get; set; }
    }
}
