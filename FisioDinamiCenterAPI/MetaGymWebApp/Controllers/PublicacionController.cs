using LogicaApp.Servicios;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
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
        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        public IActionResult Inicio()
        {
            List<PublicacionDTO> publicaciones = publicacionServicio.ObtenerPublicacionesInicio();
            return View("Inicio", publicaciones);
        }
        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        [HttpGet]
        public IActionResult MiPerfil()
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            var dto = usuarioServicio.ObtenerUsuarioGenericoDTO(usuarioId, rol);
            return View(dto);
        }

        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        [HttpPost]
        public IActionResult GestionLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);
            if (publicacionServicio.UsuarioYaDioLikePublicacion(id,usuarioId,rol))
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

                    publicacionServicio.DarLikePublicacion(id, usuarioId, rol);
                }
        public void QuitarLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            publicacionServicio.QuitarLikePublicacion(id, usuarioId, rol);
        }
        [AutorizacionRol("Admin","Profesional","Cliente")]
        [HttpPost]
        public IActionResult DarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            comentarioServicio.DarLikeComentario(id, usuarioId, rol);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [AutorizacionRol("Admin", "Profesional", "Cliente")]

        [HttpPost]
        public IActionResult QuitarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            comentarioServicio.QuitarLikeComentario(id, usuarioId, rol);
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

            comentarioServicio.AgregarComentario(comentario);

            return RedirectToAction("Inicio");
        }

        [HttpGet]
        public IActionResult Novedades()
        {
            List<PublicacionDTO> Publicaciones = publicacionServicio.ObtenerNovedades();
            return View(Publicaciones);
        }
    }
}
