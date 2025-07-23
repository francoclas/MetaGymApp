using Microsoft.AspNetCore.Mvc;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Extra;
using MetaGymWebApp.Filtros;
using MetaGymWebApp;
using LogicaNegocio.Interfaces.Servicios;

namespace WebApp.Controllers
{
    [AutorizacionRol("Admin", "Profesional", "Cliente")]
    public class NotificacionController : Controller
    {
        private readonly INotificacionServicio _notiServicio;

        public NotificacionController(INotificacionServicio notiServicio)
        {
            _notiServicio = notiServicio;
        }

        // GET: /Notificacion
        public IActionResult Index()
        {
            var usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);

            var notificaciones = _notiServicio.ObtenerPorUsuario(usuarioId, rol);
            return View(notificaciones);
        }

        // POST: /Notificacion/MarcarComoLeida/5
        [HttpPost]
        public IActionResult MarcarComoLeida(int id)
        {
            _notiServicio.MarcarComoLeida(id);
            return Ok();
        }

        // POST: /Notificacion/MarcarTodasComoLeidas
        [HttpPost]
        public IActionResult MarcarTodasComoLeidas()
        {
            var usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);

            _notiServicio.MarcarTodasComoLeidas(usuarioId, rol);
            return Ok();
        }

        // GET: /Notificacion/NoLeidasCount
        public IActionResult NoLeidasCount()
        {
            var usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);

            var cantidad = _notiServicio.ContarNoLeidas(usuarioId, rol);
            return Json(cantidad);
        }

        // GET: /Notificacion/Ultimas?tipo=Publicacion
        public IActionResult Ultimas(Enum_TipoNotificacion? tipo = null)
        {
            var usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);

            var ultimas = _notiServicio.ObtenerUltimas(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }

        public IActionResult NoLeidas()
        {
            var usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);

            var ultimas = _notiServicio.ObtenerNoLeidasUsuario(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }
        public IActionResult Leidas()
        {
            var usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rol = GestionSesion.ObtenerRol(HttpContext);

            var ultimas = _notiServicio.ObtenerLeidasUsuario(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }
    }
}
