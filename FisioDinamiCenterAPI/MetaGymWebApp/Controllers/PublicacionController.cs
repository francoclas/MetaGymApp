using LogicaApp.Servicios;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    // inicio, likes y comentarios de publicaciones
    public class PublicacionController : Controller
    {
        // Servicios que usa el módulo de publicaciones
        private readonly IPublicacionServicio _publicacionServicio;
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IComentarioServicio _comentarioServicio;

        // Inyección de dependencias
        public PublicacionController(IPublicacionServicio publicacionServicio, IUsuarioServicio usuarioServicio, IComentarioServicio comentarioServicio)
        {
            this._publicacionServicio = publicacionServicio;
            this._usuarioServicio = usuarioServicio;
            this._comentarioServicio = comentarioServicio;
        }

        // Vista base (no carga nada por ahora)
        public IActionResult Index()
        {
            return View();
        }

        // Inicio del feed (logueado con cualquier rol)
        [AutorizacionRol("Admin", "Profesional", "Cliente")]
        public IActionResult Inicio()
        {
            // Traigo publicaciones para portada/inicio
            List<PublicacionDTO> publicaciones = _publicacionServicio.ObtenerPublicacionesInicio();
            // Paso el rol para que la vista decida qué mostrar
            ViewBag.Rol = GestionSesion.ObtenerRol(HttpContext);
            return View("Inicio", publicaciones);
        }

        // Perfil propio desde publicaciones (atajo)
        [AutorizacionRol("Admin", "Profesional", "Cliente")]
        [HttpGet]
        public IActionResult MiPerfil()
        {
            // Resuelvo usuario y rol actual
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            // DTO unificado para la vista de perfil
            var dto = _usuarioServicio.ObtenerUsuarioGenericoDTO(usuarioId, rol);
            return View(dto);
        }

        // =======================
        // Likes de publicaciones
        // =======================

        // Toggle de like: si ya dio like -> lo quita; si no -> lo da
        [AutorizacionRol("Admin", "Profesional", "Cliente")]
        [HttpPost]
        public IActionResult GestionLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            if (_publicacionServicio.UsuarioYaDioLikePublicacion(id, usuarioId, rol))
            {
                QuitarLike(id);
            }
            else
            {
                DarLike(id);
            }
            // Vuelvo al inicio del feed
            return RedirectToAction("Inicio");
        }

        // Da like (métodos separados por si se usan directo desde la vista)
        public void DarLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _publicacionServicio.DarLikePublicacion(id, usuarioId, rol);
        }

        // Quita like
        public void QuitarLike(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _publicacionServicio.QuitarLikePublicacion(id, usuarioId, rol);
        }

        // =======================
        // Likes de comentarios
        // =======================

        // Like a un comentario (vuelve a la misma página usando Referer)
        [AutorizacionRol("Admin", "Profesional", "Cliente")]
        [HttpPost]
        public IActionResult DarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _comentarioServicio.DarLikeComentario(id, usuarioId, rol);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // Quitar like de un comentario
        [AutorizacionRol("Admin", "Profesional", "Cliente")]
        [HttpPost]
        public IActionResult QuitarLikeComentario(int id)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            _comentarioServicio.QuitarLikeComentario(id, usuarioId, rol);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // =======================
        // Comentarios
        // =======================

        // Agregar comentario (soporta hilo con comentarioPadreId)
        [AutorizacionRol("Admin", "Profesional", "Cliente")]
        [HttpPost]
        public IActionResult AgregarComentario(int publicacionId, string contenido, int? comentarioPadreId)
        {
            int usuarioId = GestionSesion.ObtenerUsuarioId(HttpContext);
            string rol = GestionSesion.ObtenerRol(HttpContext);

            // Mínimo necesario para registrar comentario
            ComentarioDTO comentario = new ComentarioDTO
            {
                PublicacionId = publicacionId,
                Contenido = contenido,
                AutorId = usuarioId,
                RolAutor = rol,
                ComentarioPadreId = comentarioPadreId
            };

            _comentarioServicio.AgregarComentario(comentario);

            // Vuelvo al feed
            return RedirectToAction("Inicio");
        }

        // =======================
        // Novedades públicas
        // =======================

        [HttpGet]
        public IActionResult Novedades()
        {
            // Publicaciones estilo novedades
            List<PublicacionDTO> Publicaciones = _publicacionServicio.ObtenerNovedades();
            return View(Publicaciones);
        }

        // =======================
        // Moderación de comentarios (solo Admin)
        // =======================

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