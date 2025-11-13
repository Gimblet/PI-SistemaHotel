using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaHotal.Models.Back;

namespace SistemaHotal.Controllers
{
    public class AlquilerController : Controller
    {
        public readonly IConfiguration _config;
        public AlquilerController(IConfiguration config)
        {
            _config = config;
        }
        IEnumerable<Alquiler> ListadoAlquiler()
        {
            List<Alquiler> alquilers = new List<Alquiler>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("Select * from alquiler");
                cn.Open();
                SqlDataReader dr= cmd.ExecuteReader();
                while (dr.Read()) {
                    alquilers.Add(new Alquiler()
                    {
                        id = dr.GetInt32(0),
                        idHab = new Habitacion() { id = dr.GetInt32(1) },
                        idCli = new Cliente() { id = dr.GetInt32(2) },
                        dias = dr.GetInt32(3),
                        fechaActual = dr.GetDateTime(4),
                        fechaEntrada = dr.GetDateTime(5),
                        fechaSalida = dr.GetDateTime(6),
                    });
                }
            }
            return alquilers;
        }

        public async Task<IActionResult> Index()
        {
            return View(await Task.Run(() => ListadoAlquiler()));
        }
    }
}
