using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;
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
        private readonly IAdminServicio _adminServicio;
        private readonly IProfesionalServicio _profesionalServicio;
        private readonly IClienteServicio _clienteServicio;
        public AdminController(IUsuarioServicio u, IExtraServicio extraServicio, IAdminServicio adminServicio, IProfesionalServicio profesionalServicio, IClienteServicio clienteServicio)
        {
            _usuarioServicio = u;
            _extraServicio = extraServicio;
            _adminServicio = adminServicio;
            _profesionalServicio = profesionalServicio;
            _clienteServicio = clienteServicio;
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
        public IActionResult CrearUsuario(string Ci, string Usuario, string NombreCompleto, string Correo, string Password, string Rol, string Telefono)
        {
            //Valido
            if (!ModelState.IsValid) return View();
            //Menu para cada rol
            try
            {
                switch (Rol)
                {
                    case "Admin":
                        _usuarioServicio.CrearAdmin(Ci, Usuario, NombreCompleto, Correo, Password, Telefono);
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
        public IActionResult CrearEstablecimiento(string NombreEstablecimiento, String Direccion)
        {
            if (string.IsNullOrEmpty(NombreEstablecimiento) || string.IsNullOrEmpty(Direccion))
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
        public IActionResult CrearEspecialidad(string NombreEspecialidad, String DescripcionEspecialidad)
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


        [HttpGet]
        public IActionResult GestionUsuarios()
        {
            //Obtengo lista
            List<UsuarioGenericoDTO> salida = ObtenerUsuariosListado();
            //Devuelvo
            return View(salida);
        }
        [HttpGet]
        public IActionResult EditarUsuario(int id, string rol)
        {
            return rol switch
            {
                "Cliente" => RedirectToAction("EditarCliente", new { id }),
                "Profesional" => RedirectToAction("EditarProfesional", new { id }),
                "Admin" => RedirectToAction("EditarAdmin", new { id }),
                _ => RedirectToAction("GestionUsuarios")
            };
        }
        //Editar cliente
        [HttpGet]
                public IActionResult EditarCliente(int id)
                {
                    return View();
                }

        //Editar pro
                [HttpGet]
                public IActionResult EditarProfesional(int id)
                {
                    //Obtengo especialidades        
                    ViewBag.EspecialidadesDisponibles = _extraServicio.ObtenerEspecialidades();
                    //Obtengo profesional
                    Profesional aux = _profesionalServicio.ObtenerProfesional(id);
                    UsuarioGenericoDTO profesional = new UsuarioGenericoDTO { Id = aux.Id ,
                        Usuario = aux.NombreUsuario,
                        Nombre = aux.NombreCompleto,
                        Correo = aux.Correo,
                        Pass = aux.Pass,
                        Telefono = aux.Telefono,
                        Especialidades = aux.Especialidades};
                    return View(profesional);
                }
                [HttpPost]
                public IActionResult GuardarEdicionProfesional(UsuarioGenericoDTO dto, int especialidadId)
                {
                    var profesional = _profesionalServicio.ObtenerProfesional(dto.Id); // Traélo con sus especialidades

                    // Mapear cambios
                    profesional.NombreCompleto = dto.Nombre;
                    profesional.Correo = dto.Correo;
                    if (!string.IsNullOrEmpty(dto.Pass))
                    {
                        profesional.Pass = dto.Pass;

                    }
                    profesional.Telefono = dto.Telefono;
                    if (especialidadId != 0)
                    {
                        var especialidad = _extraServicio.ObtenerEspecialidad(especialidadId);
                        _profesionalServicio.AgregarEspecialidad(especialidad, profesional);
                    }
                    else
                    {
                        _profesionalServicio.ActualizarProfesional(profesional);
                    }

                    return RedirectToAction("GestionUsuarios");
                }
        //Eliminar especialidades
        [HttpPost]
        public IActionResult EliminarEspecialidadProfesional(int profesionalId, int especialidadId)
        {
            var profesional = _profesionalServicio.ObtenerProfesional(profesionalId);
            var especialidad = _extraServicio.ObtenerEspecialidad(especialidadId);

            _profesionalServicio.EliminarEspecialidad(especialidad, profesional);

            return RedirectToAction("EditarUsuario", new { id = profesionalId, rol = "Profesional" });
        }
        //Editar admin
        [HttpGet]
                public IActionResult EditarAdmin(int id)
                {
                    return View();
                }


        private List<UsuarioGenericoDTO> ObtenerUsuariosListado()
        {
            var clientes = _clienteServicio.ObtenerTodos()
            .Select(c => new UsuarioGenericoDTO { Id = c.Id, Usuario = c.NombreUsuario, Nombre = c.NombreCompleto, Correo = c.Correo, Rol = "Cliente" });

            var profesionales = _profesionalServicio.ObtenerTodos()
                .Select(p => new UsuarioGenericoDTO { Id = p.Id, Usuario = p.NombreUsuario, Nombre = p.NombreCompleto, Correo = p.Correo, Rol = "Profesional" });

            var admins = _adminServicio.ObtenerTodos()
                .Select(a => new UsuarioGenericoDTO { Id = a.Id, Usuario = a.NombreUsuario, Nombre = a.NombreUsuario, Correo = a.Correo, Rol = "Admin" });

            var modelo = clientes.Concat(profesionales).Concat(admins).ToList();
            return modelo;
        }
    }
}
