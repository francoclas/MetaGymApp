using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    public class PublicacionController : Controller
    {   
        private readonly IPublicacionServicio publicacionServicio;
        private readonly IUsuarioServicio usuarioServicio;

        public PublicacionController(IPublicacionServicio publicacionServicio, IUsuarioServicio usuarioServicio)
        {
            this.publicacionServicio = publicacionServicio;
            this.usuarioServicio = usuarioServicio;
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
    }
}
