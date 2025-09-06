using System.Diagnostics;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    // Controlador general para vistas públicas, login/registro y perfil de usuario
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IMediaServicio _mediaServicio;
        private readonly INotificacionServicio _notificacionServicio;

        // Constructor con inyección de servicios
        public HomeController(ILogger<HomeController> logger, IUsuarioServicio usuario, IMediaServicio mediaServicio, INotificacionServicio notificacion)
        {
            _logger = logger;
            _usuarioServicio = usuario;
            _mediaServicio = mediaServicio;
            _notificacionServicio = notificacion;
        }

        // ========================
        // Vistas públicas
        // ========================

        public IActionResult Index()
        {
            return View("AcercaDe");
        }

        public IActionResult PoliticaPrivacidad()
        {
            return View("~/Views/Legal/PoliticaPrivacidad.cshtml");
        }

        public IActionResult TerminosUso()
        {
            return View("~/Views/Legal/TerminosUso.cshtml");
        }

        // ========================
        // Login
        // ========================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDTO login)
        {
            try
            {
                // Validación básica
                if (string.IsNullOrEmpty(login.Password))
                    throw new Exception("Verifique ingresar la contraseña.");
                if (string.IsNullOrEmpty(login.NombreUsuario))
                    throw new Exception("Verifique ingresar el usuario.");

                // Autenticación por servicio
                SesionDTO sesion = _usuarioServicio.IniciarSesion(login);
                GestionSesion.SetearSesion(HttpContext, sesion);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View();
            }
            return RedirectToAction("Inicio", "Publicacion");
        }

        // ========================
        // Registro de usuarios
        // ========================

        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarUsuario(ClienteDTO cliente)
        {
            try
            {
                if (cliente.Password != cliente.ConfPass)
                    throw new Exception("La confirmacion no coincide.");

                _usuarioServicio.RegistrarCliente(cliente);

                TempData["Mensaje"] = "Se registro su usuario correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Login", "Home");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View("RegistrarUsuario", cliente);
            }
        }

        // ========================
        // Perfil y edición
        // ========================

        [HttpGet]
        public IActionResult EditarPerfil()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            var dto = _usuarioServicio.ObtenerUsuarioGenericoDTO(usuarioId, rol);
            return View(dto);
        }

        [HttpPost]
        public IActionResult GuardarCambiosPerfil(UsuarioGenericoDTO dto)
        {
            try
            {
                _usuarioServicio.GuardarCambiosGenerales(dto);
                TempData["Mensaje"] = "Se modificaron datos correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("EditarPerfil");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarPerfil");
            }
        }

        [HttpPost]
        public IActionResult EliminarMediaPerfil(int mediaId)
        {
            _mediaServicio.EliminarMedia(mediaId);
            return RedirectToAction("EditarPerfil");
        }

        [HttpPost]
        public IActionResult ActualizarFotoPerfil(IFormFile archivo)
        {
            if (archivo == null)
            {
                TempData["Mensaje"] = "Debe seleccionar una imagen a cargar.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarPerfil");
            }

            // Validación de tipo MIME
            string[] tiposValidos = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!tiposValidos.Contains(archivo.ContentType))
            {
                TempData["Mensaje"] = "El archivo debe ser una imagen (jpg, png, gif, webp).";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarPerfil");
            }

            // Validación de extensión
            string[] extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            string extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!extensionesValidas.Contains(extension))
            {
                TempData["Mensaje"] = "Extensión de archivo inválida. Solo se permiten imágenes.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarPerfil");
            }

            // Rol determina tipo de entidad para guardar
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);
            var tipo = rol switch
            {
                "Cliente" => Enum_TipoEntidad.Cliente,
                "Profesional" => Enum_TipoEntidad.Profesional,
                "Admin" => Enum_TipoEntidad.Admin,
                _ => throw new Exception("Rol desconocido.")
            };

            _mediaServicio.GuardarArchivo(archivo, tipo, usuarioId);
            return RedirectToAction("EditarPerfil");
        }

        [HttpPost]
        //Se obtiene el id de la imagen y se marca como favorita que es para perfil
        public IActionResult AsignarComoFavorita(int mediaId)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);
            var tipo = rol switch
            {
                "Cliente" => Enum_TipoEntidad.Cliente,
                "Profesional" => Enum_TipoEntidad.Profesional,
                "Admin" => Enum_TipoEntidad.Admin,
                _ => throw new Exception("Rol desconocido.")
            };

            _usuarioServicio.AsignarFotoFavorita(mediaId, tipo, usuarioId);
            return RedirectToAction("EditarPerfil");
        }
        //A partir del ID se bloquea el usuario
        [HttpPost]
        public IActionResult DeshabilitarUsuario(int usuarioId, string rol, string password)
        {
            try
            {
                _usuarioServicio.DeshabilitarUsuario(usuarioId, rol, password);
                HttpContext.Session.Clear();
                TempData["Mensaje"] = "Tu cuenta fue deshabilitada correctamente. Si quisieras volver a tener acceso, debes comunicarte con un administrador de MetaGym.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"No se pudo deshabilitar la cuenta: {ex.Message}";
                return RedirectToAction("EditarPerfil");
            }
        }

        // ========================
        // Perfil con notificaciones
        // ========================

        [AutorizacionRol("Cliente", "Profesional", "Admin")]
        [HttpGet]
        public IActionResult MiPerfil()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            UsuarioGenericoDTO dto = _usuarioServicio.ObtenerUsuarioGenericoDTO(usuarioId, rol);
            dto.Notificaciones = _notificacionServicio.ObtenerPorUsuario(usuarioId, rol);

            // Armo lista de tipos de notificación disponibles
            List<string> tipos = new List<string>();
            foreach (var item in Enum.GetValues(typeof(Enum_TipoNotificacion)))
            {
                tipos.Add(item.ToString());
            }
            ViewBag.TiposNotificacion = tipos;

            return View(dto);
        }

        // ========================
        // Sesión / logout
        // ========================

        [HttpGet]
        public IActionResult Logout()
        {
            GestionSesion.CerrarSesion(HttpContext);
            return View("AcercaDe");
        }

        // ========================
        // vistas generales
        // ========================

        [HttpGet]
        public IActionResult AcercaDe()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Publicaciones()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Comunidad()
        {
            return View();
        }
    }
}