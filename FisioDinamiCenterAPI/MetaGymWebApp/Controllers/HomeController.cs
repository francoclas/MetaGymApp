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
            return View("AcercaDe");
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
            
            try
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
                SesionDTO sesion = _usuarioServicio.IniciarSesion(login);
                GestionSesion.SetearSesion(HttpContext, sesion);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegistrarUsuario(ClienteDTO cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View();
                if (cliente.Password != cliente.ConfPass)
                {
                    throw new Exception("La confirmacion no coincide.");
                }
                //Mando a Servicio
                TempData["Mensaje"] = "Se registro su usuario correctamente";
                TempData["TipoMensaje"] = "success";
                _usuarioServicio.RegistrarCliente(cliente);
                return RedirectToAction("Login", "Home");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View("RegistrarUsuario", cliente);
            }
            //Valido datos ingresados

        }
        [HttpGet]
        public IActionResult Logout()
        {
            //Limpio
            GestionSesion.CerrarSesion(HttpContext);
            return View("AcercaDe");
        }
        [HttpGet]
        public IActionResult AcercaDe()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Publicaciones()
        {
            //Pasar lista de publicaciones para que muestre
            return View();
        }
        [HttpGet]
        public IActionResult Comunidad()
        {
            return View();
        }
    }
}
