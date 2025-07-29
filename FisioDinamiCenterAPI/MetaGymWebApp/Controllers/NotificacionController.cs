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
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            List<NotificacionDTO> notificaciones = _notiServicio.ObtenerPorUsuario(usuarioId, rol);
            return View(notificaciones);
        }

        [HttpPost]
        public IActionResult MarcarComoLeida(int id)
        {
            _notiServicio.MarcarComoLeida(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult MarcarTodasComoLeidas()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _notiServicio.MarcarTodasComoLeidas(usuarioId, rol);
            return Ok();
        }

        public IActionResult NoLeidasCount()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);
            //Obtengo la cantidad desde el repo
            int cantidad = _notiServicio.ContarNoLeidas(usuarioId, rol);
            return Json(cantidad);
        }

        public IActionResult Ultimas(Enum_TipoNotificacion? tipo = null)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);
            //Obtengo notificaciones desde repo
            List<NotificacionDTO> ultimas = _notiServicio.ObtenerUltimas(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }

        public IActionResult NoLeidas()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);
            //Obtengo notificaciones desde repo
            List<NotificacionDTO> ultimas = _notiServicio.ObtenerNoLeidasUsuario(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }
        public IActionResult Leidas()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            //Obtengo notificaciones desde repo
            List<NotificacionDTO> ultimas = _notiServicio.ObtenerLeidasUsuario(usuarioId, rol);
            return PartialView("_MenuNotificaciones", ultimas);
        }
    }
}
