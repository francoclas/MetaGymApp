using Microsoft.AspNetCore.Mvc;

namespace APIClienteMetaGym.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
