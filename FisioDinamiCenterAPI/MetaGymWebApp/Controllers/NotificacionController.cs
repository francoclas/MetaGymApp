using Microsoft.AspNetCore.Mvc;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Extra;
using MetaGymWebApp.Filtros;
using MetaGymWebApp;
using LogicaNegocio.Interfaces.Servicios;

namespace WebApp.Controllers
{
    // Controlador de notificaciones (visible para Admin, Profesional y Cliente)
    [AutorizacionRol("Admin", "Profesional", "Cliente")]
    public class NotificacionController : Controller
    {
        // Servicio que maneja toda la lógica de notificaciones
        private readonly INotificacionServicio _notiServicio;

        // Inyección del servicio
        public NotificacionController(INotificacionServicio notiServicio)
        {
            _notiServicio = notiServicio;
        }

        // Página principal de notificaciones del usuario logueado
        // GET: /Notificacion
        public IActionResult Index()
        {
            // Identifico usuario y rol desde la sesión
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            // Traigo todas sus notificaciones (leídas y no leídas)
            List<NotificacionDTO> notificaciones = _notiServicio.ObtenerPorUsuario(usuarioId, rol);
            return View(notificaciones);
        }

        // Marca una notificación puntual como leída (llamado vía AJAX normalmente)
        [HttpPost]
        public IActionResult MarcarComoLeida(int id)
        {
            _notiServicio.MarcarComoLeida(id);
            return Ok();
        }

        // Marca todas las notificaciones del usuario actual como leídas
        [HttpPost]
        public IActionResult MarcarTodasComoLeidas()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _notiServicio.MarcarTodasComoLeidas(usuarioId, rol);
            return Ok();
        }

        // Devuelve la cantidad de no leídas (útil para el badge en el header)
        public IActionResult NoLeidasCount()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            // Pido el contador al servicio
            int cantidad = _notiServicio.ContarNoLeidas(usuarioId, rol);
            return Json(cantidad);
        }

        // Devuelve las últimas notificaciones (para desplegable del menú)
        public IActionResult Ultimas(Enum_TipoNotificacion? tipo = null)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            // Traigo últimas (independiente del filtro por ahora, igual que tu código)
            List<NotificacionDTO> ultimas = _notiServicio.ObtenerUltimas(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }

        // Lista solo no leídas en el menú (si la sesión expiró, muestro parcial de sesión expirada)
        public IActionResult NoLeidas()
        {
            SesionDTO sesion = GestionSesion.ObtenerSesion(HttpContext);
            if (sesion == null)
            {
                return PartialView("_SesionExpirada");
            }

            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            // Traigo NO leídas
            List<NotificacionDTO> ultimas = _notiServicio.ObtenerNoLeidasUsuario(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }

        // Lista solo leídas en el menú
        public IActionResult Leidas()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            // Traigo leídas
            List<NotificacionDTO> ultimas = _notiServicio.ObtenerLeidasUsuario(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }
    }
}