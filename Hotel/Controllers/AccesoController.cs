using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.ViewModels;
using Hotel.Models;
using Hotel.Filtro;

namespace Hotel.Controllers
{
    [AutenticacionFilter]
    public class AccesoController : Controller
    {
        private readonly HotelContext _context;

        public AccesoController(HotelContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var acceso = from u in _context.Acceso
                          join c in _context.Cliente on u.idCli equals c.id
                          join t in _context.TipoDoc on c.tipoId equals t.id
                          select new AccesoFrontEnd
                          {
                              id = u.id,
                              idCli = c.id,
                              nombre = c.nombre,
                              apellido = c.apellido,
                              tipoId = t.id,
                              tipoDoc = t.tipo,
                              numeroDoc = c.numeroDoc,
                              telefono = c.telefono,
                              usuario=u.usuario,
                              clave=u.clave,
                              tipoUsuario=u.tipoUsuario
                          };
            return View(await acceso.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo");
            ViewBag.tipoUser = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Administrador", Value = "ADMIN" },
                new SelectListItem { Text = "Empleado", Value = "EMPL" }
            }, "Value", "Text");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccesoFrontEnd modelo)
        {
            var clienteExistente = await _context.Cliente.FirstOrDefaultAsync(c => c.numeroDoc == modelo.numeroDoc);
            if (clienteExistente == null)
            {
                var cliente = new Cliente
                {
                    nombre = modelo.nombre,
                    apellido = modelo.apellido,
                    numeroDoc = modelo.numeroDoc,
                    tipoId = modelo.tipoId,
                    telefono = modelo.telefono
                };
                _context.Cliente.Add(cliente);
                await _context.SaveChangesAsync();

                clienteExistente = cliente;
            }
            modelo.idCli = clienteExistente.id;
            if (modelo.clave != modelo.repetirClave)
            {
                ViewData["Mensaje"] = "Las Contraseñas deben coincidir";
                ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo", modelo.tipoId);
                errores(modelo);
                return View(modelo);
            }
            Acceso? uEncontrado = await _context.Acceso.Where(a => a.usuario == modelo.usuario).FirstOrDefaultAsync();
            if (uEncontrado != null)
            {
                ViewData["Mensaje"] = "Este usuario Ya se encuentra en uso; Por Favor Intenta con otro Nombre de usuario.";
                ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo", modelo.tipoId);
                errores(modelo);
                return View(modelo);
            }
            
            var acceso = new Acceso
            {
                idCli = modelo.idCli,
                usuario = modelo.usuario,
                clave = modelo.clave,
                tipoUsuario = modelo.tipoUsuario,
            };
            await _context.Acceso.AddAsync(acceso);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public void errores(AccesoFrontEnd modelo)
        {
            ViewBag.tipoUser = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Administrador", Value = "ADMIN" },
                new SelectListItem { Text = "Empleado", Value = "EMPL" }
            }, "Value", "Text", modelo.tipoUsuario);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarCliente(string numeroDoc)
        {
            var cliente = await _context.Cliente
                .Where(c => c.numeroDoc == numeroDoc)
                .Select(c => new
                {
                    c.id,
                    c.nombre,
                    c.apellido,
                    c.telefono,
                    c.tipoId
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
                return Json(new { success = false, message = "Cliente no encontrado." });

            return Json(new { success = true, cliente });
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await (from a in _context.Acceso
                                 join c in _context.Cliente on a.idCli equals c.id
                                 join t in _context.TipoDoc on c.tipoId equals t.id
                                 where a.id == id
                                 select new AccesoFrontEnd
                                 {
                                     id = a.id,
                                     idCli = c.id,
                                     nombre = c.nombre,
                                     apellido = c.apellido,
                                     tipoId = t.id,
                                     tipoDoc = t.tipo,
                                     numeroDoc = c.numeroDoc,
                                     telefono = c.telefono,
                                     usuario=a.usuario,
                                     clave=a.clave,
                                     tipoUsuario=a.tipoUsuario
                                 }).FirstOrDefaultAsync();
            if (cliente == null)
            {
                return NotFound();
            }

            ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo", cliente.tipoId);
            errores(cliente);
            return View(cliente);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AccesoFrontEnd acceso)
        {
            if (id != acceso.id)
            {
                return NotFound();
            }
            if (acceso.clave != acceso.repetirClave)
            {
                ViewData["Mensaje"] = "Las Contraseñas deben coincidir";
                errores(acceso);
                return View(acceso);
            }
            Acceso? uEncontrado = await _context.Acceso.Where(a => a.usuario == acceso.usuario && a.id!=id).FirstOrDefaultAsync();
            if (uEncontrado != null)
            {
                ViewData["Mensaje"] = "Este usuario Ya se encuentra en uso; Por Favor Intenta con otro Nombre de usuario.";
                errores(acceso);
                return View(acceso);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var acc = await _context.Acceso.FindAsync(acceso.id);
                    if (acc == null) { return NotFound(); }
                    acc.usuario = acceso.usuario;
                    acc.clave = acceso.clave;
                    acc.tipoUsuario = acceso.tipoUsuario;
                    _context.Update(acc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccesoExists(acceso.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            errores(acceso);
            return View(acceso);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acceso = await _context.Acceso
                .FirstOrDefaultAsync(m => m.id == id);
            if (acceso == null)
            {
                return NotFound();
            }

            return View(acceso);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acceso = await _context.Acceso.FindAsync(id);
            if (acceso != null)
            {
                _context.Acceso.Remove(acceso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccesoExists(int id)
        {
            return _context.Acceso.Any(e => e.id == id);
        }
    }
}
