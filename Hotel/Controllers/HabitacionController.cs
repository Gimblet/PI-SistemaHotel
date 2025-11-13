using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.Models;
using System.Diagnostics;
using Hotel.ViewModels;
using Hotel.Filtro;
using System.Text.Json;

namespace Hotel.Controllers
{
    [AutenticacionFilter]
    public class HabitacionController : Controller
    {
        private readonly HotelContext _context;
        /*
        public HabitacionController(HotelContext context)
        {
            _context = context;
        }
        */
        private readonly ILogger<HabitacionController> _logger;
        public HabitacionController(ILogger<HabitacionController> logger, HotelContext context)
        {
            _logger = logger;
            _context = context;
        }
        // GET: Habitacion
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("ESTO ES UNA PRUEBA");
            var habitacionesBase = await (from h in _context.Habitacion
                                          join t in _context.TipoHab on h.TipoId equals t.id
                                          join e in _context.Estado on h.EstadoId equals e.id
                                          select new
                                          {
                                              h.Id, h.Numero,
                                              t.tipo, t.descripcion,
                                              t.precio, h.EstadoId,
                                              e.estado
                                          }).ToListAsync();
            var ultimosAlquileres = await _context.Alquiler
                .GroupBy(a => a.idHab).Select(g => new
                                        {
                                            IdHab = g.Key,
                                            UltimoAlquiler = g.OrderByDescending(a => a.id).FirstOrDefault()
                                        }).ToListAsync();
            var habitaciones = (from h in habitacionesBase
                                      join a in ultimosAlquileres on h.Id equals a.IdHab into alq
                                      from ultimoAlquiler in alq.DefaultIfEmpty()
                                      select new HabitacionFrontEnd
                                      {
                                          Id = h.Id,
                                          Numero = h.Numero,
                                          TipoHabitacion = h.tipo,
                                          Descripcion = h.descripcion,
                                          precio = h.precio,
                                          EstadoActualId = h.EstadoId,
                                          Estados = h.estado,
                                          fechaSalida = ultimoAlquiler?.UltimoAlquiler.fechaSalida
                                      }).ToList();
            var habitacionesAgrupadas = habitaciones
                .GroupBy(h => h.Numero?.Substring(0, 1)).OrderBy(g => g.Key).ToList();
            ViewBag.HabitacionesAgrupadas = habitacionesAgrupadas;

            ViewData["Estados"] = new SelectList(await _context.Estado.ToListAsync(), "id", "estado");

            var nombreUsuario = HttpContext.Session.GetString("nombre");

            ViewBag.nombre = nombreUsuario;
            _logger.LogInformation("Habitaciones Creadas: "+JsonSerializer.Serialize(habitaciones));
            return View(habitaciones);
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarEstados(int habitacionId, int estadoId)
        {
            var habitacionDb = await _context.Habitacion.FindAsync(habitacionId);
            if (habitacionDb != null)
            {
                habitacionDb.EstadoId = estadoId;
                _context.Update(habitacionDb);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpGet]
        public async Task<IActionResult> RegistrarAlquiler(int id)
        {
            var modelo = await (from h in _context.Habitacion
                                join t in _context.TipoHab on h.TipoId equals t.id
                                where h.Id == id
                                select new AlquilerFrontEnd
                                {
                                    HabitacionId = id,
                                    Numero = h.Numero,
                                    TipoHabitacion = t.tipo,
                                    Descripcion = t.descripcion,
                                    Precio = t.precio,
                                    FechaIngreso=DateTime.Now,
                                }).FirstOrDefaultAsync();
            if (modelo == null)
            {
                return NotFound();
            }

            ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo");
            return View(modelo);
        }
        
        [HttpPost]
        public async Task<IActionResult> RegistrarAlquiler(AlquilerFrontEnd modelo)
        {
            var habitacion = await _context.Habitacion
                .Join(_context.TipoHab, h => h.TipoId, t => t.id, (h, t) => new { h, t })
                .Where(x => x.h.Id == modelo.HabitacionId)
                .Select(x => new
                {
                    x.h.Id,
                    x.h.Numero,
                    x.t.tipo,
                    x.t.descripcion,
                    x.t.precio
                })
                .FirstOrDefaultAsync();

            if (habitacion == null)
            {
                if (modelo.HabitacionId == 0)
                {
                    return NotFound("El ID de la habitación no fue enviado correctamente."+modelo.HabitacionId);
                }
                return NotFound("La habitación ya no está disponible.");
            }

            modelo.HabitacionId = habitacion.Id;
            modelo.Numero = habitacion.Numero;
            modelo.TipoHabitacion = habitacion.tipo;
            modelo.Descripcion = habitacion.descripcion;
            modelo.Precio = habitacion.precio;


            if (!ModelState.IsValid)
            {
                ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo");
                return View(modelo);
            }



            var clienteExistente = await _context.Cliente.FirstOrDefaultAsync(c => c.numeroDoc == modelo.Documento);
            if (clienteExistente == null)
            {
                var cliente = new Cliente
                {
                    nombre = modelo.Nombre,
                    apellido = modelo.Apellido,
                    numeroDoc = modelo.Documento,
                    tipoId = modelo.tipoDocId,
                    telefono = modelo.Telefono
                };
                _context.Cliente.Add(cliente);
                await _context.SaveChangesAsync();

                clienteExistente = cliente;
            }
            else
            {
                modelo.idCli = clienteExistente.id;
                modelo.Nombre = clienteExistente.nombre;
                modelo.Apellido = clienteExistente.apellido;
                modelo.Telefono = clienteExistente.telefono;
            }

            var alquiler = new Alquiler
            {
                idHab = modelo.HabitacionId,
                idCli = clienteExistente.id,
                dias = modelo.dias,
                fechaActual = DateTime.Now,
                fechaEntrada = DateTime.Now,
                fechaSalida = DateTime.Now.AddDays(modelo.dias),
                total= modelo.Precio * modelo.dias,
            };
            _context.Alquiler.Add(alquiler);
            await _context.SaveChangesAsync();

            var hab = await _context.Habitacion.FindAsync(habitacion.Id);
            if (hab != null)
            {
                hab.EstadoId = 2;
                _context.Update(hab);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> BuscarCliente(string numeroDocumento)
        {
            if (string.IsNullOrEmpty(numeroDocumento))
                return BadRequest("El número de documento es requerido.");

            var cliente = await _context.Cliente
                .Where(c => c.numeroDoc == numeroDocumento)
                .Select(c => new
                {
                    c.nombre,
                    c.apellido,
                    c.telefono,
                    c.tipoId
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            return Ok(cliente);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
