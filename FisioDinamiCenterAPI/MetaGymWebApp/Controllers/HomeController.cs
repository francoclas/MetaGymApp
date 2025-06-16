using System.Diagnostics;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioServicio _usuarioServicio;
        public HomeController(ILogger<HomeController> logger,IUsuarioServicio usuario)
        {
            _logger = logger;
            _usuarioServicio = usuario; 
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });


            
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginDTO login)
        {
            //Valido Credenciales
            if (string.IsNullOrEmpty(login.Password))
            {
                throw new Exception("Verifique ingresar la contraseña.");
            }
            if (string.IsNullOrEmpty(login.NombreUsuario))
            {
                throw new Exception("Verifique ingresar el usuario.");
            }
            //Consulto 
            try
            {
                SesionDTO sesion = _usuarioServicio.IniciarSesion(login);
                GestionSesion.SetearSesion(HttpContext, sesion);
            }
            catch (Exception e)
            {
                TempDataMensaje.SetMensaje(this, e.Message, "Error");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            //Limpio
            GestionSesion.CerrarSesion(HttpContext);
            return View("Index", "Home");
        }
    }
}
