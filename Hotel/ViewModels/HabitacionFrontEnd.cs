using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.ViewModels
{
    public class HabitacionFrontEnd
    {
        public int Id { get; set; }
        public string? Numero { get; set; }
        public string? TipoHabitacion { get; set; }
        public string? Descripcion { get; set; }
        public decimal precio { get; set; }
        public int EstadoActualId { get; set; }
        public string? Estados { get; set; }
        public List<SelectListItem> EstadosDisponibles { get; set; }
        public DateTime? fechaSalida { get; set; }
    }
}
