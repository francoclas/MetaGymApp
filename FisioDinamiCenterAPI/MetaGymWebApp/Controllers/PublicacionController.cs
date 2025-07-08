using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    public class PublicacionController : Controller
    {   
        private readonly IPublicacionServicio publicacionServicio;

        public PublicacionController(IPublicacionServicio publicacionServicio)
        {
            this.publicacionServicio = publicacionServicio;
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

    }
}
