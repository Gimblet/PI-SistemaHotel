using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.ViewModels;
using Hotel.Filtro;

namespace Hotel.Controllers
{
    [AutenticacionFilter]
    public class ClienteController : Controller
    {
        private readonly HotelContext _context;

        public ClienteController(HotelContext context)
        {
            _context = context;
        }

        // GET: Cliente
        public async Task<IActionResult> Index(string buscar, string ordenar)
        {
            ViewData["NumDocSortParm"] = String.IsNullOrEmpty(ordenar) ? "num_doc_des" : "";
            ViewData["NomSortParm"] = ordenar=="Nombre" ? "nom_des" : "Nombre";
            ViewData["ApeSortParm"] = ordenar=="Apellido" ? "ape_des" : "Apellido";
            ViewData["TipSortParm"] = ordenar=="Tipo" ? "tip_des" : "Tipo";
            ViewData["TelSortParm"] = ordenar=="Telefono" ? "tel_des" : "Telefono";

            var cliente = from c in _context.Cliente
                          join t in _context.TipoDoc on c.tipoId equals t.id
                          select new ClienteFrontEnd
                          {
                              id = c.id,
                              nombre=c.nombre,
                              apellido=c.apellido,
                              tipoId=t.id,
                              tipoDoc=t.tipo,
                              numeroDoc=c.numeroDoc,
                              telefono=c.telefono
                          };
            switch (ordenar)
            {
                case "num_doc_des":
                    cliente = cliente.OrderByDescending(c => c.numeroDoc);
                    break;
                case "Nombre":
                    cliente = cliente.OrderBy(c => c.nombre);
                    break;
                case "nom_des":
                    cliente = cliente.OrderByDescending(c => c.nombre);
                    break;
                case "Apellido":
                    cliente = cliente.OrderBy(c => c.apellido);
                    break;
                case "ape_des":
                    cliente = cliente.OrderByDescending(c => c.apellido);
                    break;
                case "Tipo":
                    cliente = cliente.OrderBy(c => c.tipoDoc);
                    break;
                case "tip_des":
                    cliente = cliente.OrderByDescending(c => c.tipoDoc);
                    break;
                case "Telefono":
                    cliente = cliente.OrderBy(c => c.telefono);
                    break;
                case "tel_des":
                    cliente = cliente.OrderByDescending(c => c.telefono);
                    break;
                default:
                    cliente = cliente.OrderBy(c => c.numeroDoc);
                    break;
            }

            if (!String.IsNullOrEmpty(buscar))
            {
                cliente = cliente.Where(c => c.numeroDoc!.Contains(buscar));
            }
            return View(await cliente.ToListAsync());
        }

        // GET: Cliente/Edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await (from c in _context.Cliente
                                 join t in _context.TipoDoc on c.tipoId equals t.id
                                 where c.id == id
                                 select new ClienteFrontEnd
                                 {
                                     id = c.id,
                                     nombre = c.nombre,
                                     apellido = c.apellido,
                                     tipoId = t.id,
                                     tipoDoc = t.tipo,
                                     numeroDoc = c.numeroDoc,
                                     telefono = c.telefono
                                 }).FirstOrDefaultAsync();
            if (cliente == null)
            {
                return NotFound();
            }

            ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo", cliente.tipoId);
            return View(cliente);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nombre,apellido,tipoId,numeroDoc,telefono")] ClienteFrontEnd cliente)
        {
            if (id != cliente.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var c = await _context.Cliente.FindAsync(cliente.id);
                    if (c == null) { return NotFound(); }
                    c.nombre = cliente.nombre;
                    c.apellido = cliente.apellido;
                    c.tipoId = cliente.tipoId;
                    c.numeroDoc = cliente.numeroDoc;
                    c.telefono = cliente.telefono;
                    _context.Update(c);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.id))
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
            ViewBag.tipoDoc = new SelectList(await _context.TipoDoc.ToListAsync(), "id", "tipo", cliente.tipoId);
            return View(cliente);
        }

        // GET: Cliente/Delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Cliente/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            var usuario = await _context.Acceso.FirstOrDefaultAsync(m => m.idCli == id);
            if (cliente != null)
            {
                if (usuario == null)
                {
                    _context.Cliente.Remove(cliente);
                }
                else
                {
                    ViewData["Mensaje"] = "Éste cliente posee una cuenta de usuario, si desea eliminarlo deberá eliminar su acceso primero.";
                    return View(cliente);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.id == id);
        }
    }
}
