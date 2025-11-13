namespace Hotel.Models
{
    public class Acceso
    {
        public int id { get; set; }
        public int idCli { get; set; }
        public string? usuario { get; set; }
        public string? clave { get; set; }
        public string? tipoUsuario { get; set; }
    }
}
