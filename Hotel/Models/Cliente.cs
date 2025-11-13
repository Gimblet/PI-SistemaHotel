namespace Hotel.Models
{
    public class Cliente
    {
        public int id { get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public int tipoId { get; set; }
        public string? numeroDoc { get; set; }
        public string? telefono { get; set; }
    }
}
