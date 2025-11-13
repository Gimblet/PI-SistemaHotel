namespace SistemaHotal.Models.Back
{
    public class Habitacion
    {
        public int id { get; set; }
        public string? numero { get; set; }
        public TipoHab? tipoId { get; set; }
        public Estado? estadoId { get; set; }
    }
}
