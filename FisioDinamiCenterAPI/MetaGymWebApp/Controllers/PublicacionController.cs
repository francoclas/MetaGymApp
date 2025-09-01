using LogicaApp.Servicios;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    public class PublicacionController : Controller
    {   
        private readonly IPublicacionServicio _publicacionServicio;
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IComentarioServicio _comentarioServicio;
        public PublicacionController(IPublicacionServicio publicacionServicio, IUsuarioServicio usuarioServicio, IComentarioServicio comentarioServicio)
        {
            this._publicacionServicio = publicacionServicio;
            this._usuarioServicio = usuarioServicio;
            this._comentarioServicio = comentarioServicio;
        }

        public IActionResult Index()
        {
            return View();
        }
        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        public IActionResult Inicio()
        {
            List<PublicacionDTO> publicaciones = _publicacionServicio.ObtenerPublicacionesInicio();
            ViewBag.Rol = GestionSesion.ObtenerRol(HttpContext);
            return View("Inicio", publicaciones);
        }
        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        [HttpGet]
        public IActionResult MiPerfil()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            var dto = _usuarioServicio.ObtenerUsuarioGenericoDTO(usuarioId, rol);
            return View(dto);
        }

        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        [HttpPost]
        public IActionResult GestionLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);
            if (_publicacionServicio.UsuarioYaDioLikePublicacion(id,usuarioId,rol))
            {
                QuitarLike(id);
            }
            else
            {
                 DarLike(id);
            }
            return RedirectToAction("Inicio");
        }
        public void DarLike(int id)
               {
                    int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
                    string rol = GestionSesion.ObtenerRol(HttpContext);

                    _publicacionServicio.DarLikePublicacion(id, usuarioId, rol);
                }
        public void QuitarLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _publicacionServicio.QuitarLikePublicacion(id, usuarioId, rol);
        }
        [AutorizacionRol("Admin","Profesional","Cliente")]
        [HttpPost]
        public IActionResult DarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _comentarioServicio.DarLikeComentario(id, usuarioId, rol);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        [HttpPost]
        public IActionResult QuitarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _comentarioServicio.QuitarLikeComentario(id, usuarioId, rol);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        [HttpPost]
        public IActionResult AgregarComentario(int publicacionId, string contenido, int? comentarioPadreId)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            ComentarioDTO comentario = new ComentarioDTO
            {
                PublicacionId = publicacionId,
                Contenido = contenido,
                AutorId = usuarioId,
                RolAutor = rol,
                ComentarioPadreId = comentarioPadreId
            };

            _comentarioServicio.AgregarComentario(comentario);

            return RedirectToAction("Inicio");
        }

        [HttpGet]
        public IActionResult Novedades()
        {
            List<PublicacionDTO> Publicaciones = _publicacionServicio.ObtenerNovedades();
            return View(Publicaciones);
        }
        [AutorizacionRol("Admin")]
        [HttpPost]
        public IActionResult OcultarComentario(int comentarioId)
        {
            _publicacionServicio.OcultarComentario(comentarioId);
            TempData["Mensaje"] = "Se actualizo el comentario.";
            TempData["TipoMensaje"] = "danger";
            return RedirectToAction("Inicio");
        }
    }
}
