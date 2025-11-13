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
using Microsoft.AspNetCore.Http;

namespace Hotel.Controllers
{
    public class LoginController : Controller
    {
        private readonly HotelContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(HotelContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginVM modelo)
        {
            Acceso? usuario = await _context.Acceso.Where(a => a.usuario == modelo.usuario && a.clave == modelo.clave).FirstOrDefaultAsync();
            if (usuario == null)
            {
                ViewData["Mensaje"] = "Usuario o Contraseña Incorrecto";
                return View();
            }
            var nombre= await _context.Cliente.Where(c => c.id == usuario.idCli).FirstOrDefaultAsync();
            _httpContextAccessor.HttpContext.Session.SetString("eUsuario", usuario.usuario);
            _httpContextAccessor.HttpContext.Session.SetString("nombre", nombre.nombre +" "+ nombre.apellido);
            _httpContextAccessor.HttpContext.Session.SetString("tipoUsuario", usuario.tipoUsuario);
            return RedirectToAction("Index", "Habitacion");
        }

        [HttpGet]
        public ActionResult logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
