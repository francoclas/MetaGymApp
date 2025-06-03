using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MetaGymWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IExtraServicio _extraServicio;
        public AdminController(IUsuarioServicio u, IExtraServicio extraServicio)
        {
            _usuarioServicio = u;
            _extraServicio = extraServicio;
        }
        public IActionResult Index()
        {
            return View();
        }

        //Creacion de usuarios
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearUsuario(string Ci,string Usuario,string NombreCompleto,string Correo,string Password,string Rol, string Telefono)
        { 
            //Valido
            if (!ModelState.IsValid) return View();
            //Menu para cada rol
            try
            {
                switch (Rol)
                {
                    case "Admin":
                        _usuarioServicio.CrearAdmin(Ci, Usuario,NombreCompleto,Correo, Password, Telefono);
                        break;
                    case "Profesional":
                        _usuarioServicio.CrearProfesional(Ci, Usuario, NombreCompleto, Correo, Password, Telefono);
                        break;
                    case "Cliente":
                        _usuarioServicio.CrearCliente(Ci, Usuario, NombreCompleto, Correo, Password, Telefono);
                        break;
                }
                return RedirectToAction("CrearUsuario");
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction("CrearUsuario");

            }
        }
        [HttpPost]
        public IActionResult CrearEstablecimiento(string NombreEstablecimiento,String Direccion) {
            if(string.IsNullOrEmpty(NombreEstablecimiento) || string.IsNullOrEmpty(Direccion))
            {
                TempData["Error"] = "Debe completar todos los campos";
                return RedirectToAction("CrearUsuario");
            }
            //temporal instancio establecimiento y cargo
            Establecimiento e = new Establecimiento(NombreEstablecimiento, Direccion);
            _extraServicio.RegistrarNuevoEstablecimiento(e);
            return RedirectToAction("CrearUsuario");

        }
        [HttpPost]
        public IActionResult CrearEspecialidad(string  NombreEspecialidad,String DescripcionEspecialidad)
        {
            if (string.IsNullOrEmpty(NombreEspecialidad) || string.IsNullOrEmpty(DescripcionEspecialidad))
            {
                TempData["Error"] = "Debe completar todos los campos";
                return RedirectToAction("CrearUsuario");
            }
            //temporal instancio especialidad y cargo
            Especialidad e = new Especialidad(NombreEspecialidad, DescripcionEspecialidad);
            _extraServicio.RegistrarNuevaEspecialidad(e);
            return RedirectToAction("CrearUsuario");


        }
    }
}
