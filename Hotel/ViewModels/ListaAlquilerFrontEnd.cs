namespace Hotel.ViewModels
{
    public class ListaAlquilerFrontEnd
    {
        public int id { get; set; }
        public string? numHab { get; set; }
        public string? nombre { get; set; }
        public string? documento { get; set; }
        public int dias { get; set; }
        public decimal precio { get; set; }
        public DateTime? fechaActual { get; set; }
        public DateTime? fechaEntrada { get; set; }
        public DateTime? fechaSalida { get; set; }
        public decimal total { get; set; }
    }
}
