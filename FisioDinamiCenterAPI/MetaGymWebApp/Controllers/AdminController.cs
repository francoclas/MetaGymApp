using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MetaGymWebApp.Controllers
{
    [AutorizacionRol("Admin")]
    public class AdminController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IExtraServicio _extraServicio;
        private readonly IAdminServicio _adminServicio;
        private readonly IProfesionalServicio _profesionalServicio;
        private readonly IClienteServicio _clienteServicio;
        private readonly IMediaServicio _mediaServicio;
        public AdminController(IUsuarioServicio u, IExtraServicio extraServicio, IAdminServicio adminServicio, IProfesionalServicio profesionalServicio, IClienteServicio clienteServicio, IMediaServicio mediaServicio)
        {
            _usuarioServicio = u;
            _extraServicio = extraServicio;
            _adminServicio = adminServicio;
            _profesionalServicio = profesionalServicio;
            _clienteServicio = clienteServicio;
            _mediaServicio = mediaServicio;
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
        public IActionResult CrearUsuario(CrearUsuarioDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                switch (dto.Rol)
                {
                    case "Admin":
                        _usuarioServicio.CrearAdmin(dto.Ci, dto.Usuario, dto.NombreCompleto, dto.Correo, dto.Password, dto.Telefono);
                        break;
                    case "Profesional":
                        _usuarioServicio.CrearProfesional(dto.Ci, dto.Usuario, dto.NombreCompleto, dto.Correo, dto.Password, dto.Telefono);
                        break;
                    case "Cliente":
                        _usuarioServicio.CrearCliente(dto.Ci, dto.Usuario, dto.NombreCompleto, dto.Correo, dto.Password, dto.Telefono);
                        break;
                }

                TempData["Mensaje"] = "Usuario creado correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("CrearUsuario");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("CrearUsuario");
            }
        }
        [HttpPost]
        public IActionResult CrearEstablecimiento(string NombreEstablecimiento, String Direccion)
        {
            try
            {
                if (string.IsNullOrEmpty(NombreEstablecimiento) || string.IsNullOrEmpty(Direccion))
                {
                    throw new Exception("Debe completar todos los campos");
                    return RedirectToAction("CrearUsuario");
                }
                //temporal instancio establecimiento y cargo
                Establecimiento e = new Establecimiento(NombreEstablecimiento, Direccion);
                _extraServicio.RegistrarNuevoEstablecimiento(e);
                return RedirectToAction("CrearUsuario");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View();
            }
           

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


        //Menu general
        [HttpGet]
        public IActionResult PanelControl() { 
            PanelControlAdminModel model = new PanelControlAdminModel();
            model.Establecimientos = _extraServicio.ObtenerEstablecimientos();
            model.Especialidades = _extraServicio.ObtenerEspecialidades();  
            return View(model); 
            
        }
        
        //Seccion usuarios
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

        //Seccion extras
        [HttpGet]
        public IActionResult RegistrarEspecialidad()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarEspecialidad(EspecialidadDTO dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Datos inválidos.";
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }

            _extraServicio.RegistrarEspecialidad(dto);
            TempData["Mensaje"] = "Especialidad registrada correctamente.";
            return RedirectToAction("PanelControl");
        }

        [HttpGet]
        public IActionResult RegistrarEstablecimiento()
        {
            return View();
        }

    
        [HttpPost]
        public IActionResult RegistrarEstablecimiento(EstablecimientoDTO dto, IFormFile archivo)
        {

            //Reviso que el modelo estebien
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Datos inválidos.";
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }

            //Instancio nuevo establecimiento para mandar al sistema
            var nuevo = new Establecimiento
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion
            };

            //Envio a la bd
            _extraServicio.RegistrarNuevoEstablecimiento(nuevo); 
            //Verifico si cargaron imagen, si es asi, mando imagen al sistema
            if (archivo != null && archivo.Length > 0)
            {
                _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Establecimiento, nuevo.Id);
            }

            TempData["Mensaje"] = "Establecimiento registrado correctamente.";
            return RedirectToAction("PanelControl");
        }
        //Ediciones
        [HttpGet]
        public IActionResult EditarEstablecimiento(int id)
        {
            var entidad = _extraServicio.ObtenerEstablecimiento(id);
            if (entidad == null) return NotFound();

            var mediaUrl = entidad.Media?.FirstOrDefault()?.Url ?? "/mediaweb/default/gym_default.jpg";

            var dto = new EstablecimientoDTO
            {
                Id = entidad.Id,
                Nombre = entidad.Nombre,
                Direccion = entidad.Direccion,
                UrlMedia = mediaUrl
            };

            return View(dto);
        }

        [HttpPost]
        public IActionResult EditarEstablecimiento(EstablecimientoDTO dto, IFormFile archivo)
        {
            var establecimiento = _extraServicio.ObtenerEstablecimiento(dto.Id);
            if (establecimiento == null) return NotFound();

            establecimiento.Nombre = dto.Nombre;
            establecimiento.Direccion = dto.Direccion;
            _extraServicio.GuardarCambios();

            if (archivo != null && archivo.Length > 0)
            {
                _mediaServicio.ReemplazarArchivo(archivo, Enum_TipoEntidad.Establecimiento, establecimiento.Id);
            }

            TempData["Mensaje"] = "Establecimiento actualizado.";
            return RedirectToAction("PanelControl");
        }
        //Editar especialidades
        [HttpGet]
        public IActionResult EditarEspecialidad(int id)
        {
            var entidad = _extraServicio.ObtenerEspecialidad(id);
            if (entidad == null) return NotFound();

            var dto = new EspecialidadDTO
            {
                Id = entidad.Id,
                NombreEspecialidad = entidad.NombreEspecialidad,
                DescripcionEspecialidad = entidad.DescripcionEspecialidad
            };

            return View(dto);
        }

        [HttpPost]
        public IActionResult EditarEspecialidad(EspecialidadDTO dto)
        {
            var entidad = _extraServicio.ObtenerEspecialidad(dto.Id);
            if (entidad == null) return NotFound();

            entidad.NombreEspecialidad = dto.NombreEspecialidad;
            entidad.DescripcionEspecialidad = dto.DescripcionEspecialidad;

            _extraServicio.GuardarCambios();
            TempData["Mensaje"] = "Especialidad actualizada.";
            return RedirectToAction("PanelControl");
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
