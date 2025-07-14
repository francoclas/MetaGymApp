using Microsoft.AspNetCore.Mvc;

namespace APIClienteMetaGym.Controllers
{
    public class PublicacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
