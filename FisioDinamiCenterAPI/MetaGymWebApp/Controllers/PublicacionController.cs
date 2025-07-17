using LogicaApp.Servicios;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    public class PublicacionController : Controller
    {   
        private readonly IPublicacionServicio publicacionServicio;
        private readonly IUsuarioServicio usuarioServicio;
        private readonly IComentarioServicio comentarioServicio;
        public PublicacionController(IPublicacionServicio publicacionServicio, IUsuarioServicio usuarioServicio, IComentarioServicio comentarioServicio)
        {
            this.publicacionServicio = publicacionServicio;
            this.usuarioServicio = usuarioServicio;
            this.comentarioServicio = comentarioServicio;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Inicio()
        {
            List<PublicacionDTO> publicaciones = publicacionServicio.ObtenerPublicacionesInicio();
            return View("Inicio", publicaciones);
        }
        [HttpGet]
        public IActionResult MiPerfil()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            var dto = usuarioServicio.ObtenerUsuarioGenericoDTO(usuarioId, rol);
            return View(dto);
        }
        [HttpPost]
        public IActionResult DarLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            publicacionServicio.DarLikePublicacion(id, usuarioId, rol);
            return RedirectToAction("DetallePublicacion", new { id });
        }

        [HttpPost]
        public IActionResult QuitarLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            publicacionServicio.QuitarLikePublicacion(id, usuarioId, rol);
            return RedirectToAction("DetallePublicacion", new { id });
        }
        [HttpPost]
        public IActionResult DarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            comentarioServicio.DarLikeComentario(id, usuarioId, rol);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult QuitarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            comentarioServicio.QuitarLikeComentario(id, usuarioId, rol);
            return Redirect(Request.Headers["Referer"].ToString());
        }
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

            comentarioServicio.AgregarComentario(comentario);

            return RedirectToAction("DetallePublicacion", new { id = publicacionId });
        }

    }
}
