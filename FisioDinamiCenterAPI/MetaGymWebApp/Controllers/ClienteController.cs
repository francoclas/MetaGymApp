using LogicaApp.DTOS;
using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    public class ClienteController : Controller
    {   
        private readonly IUsuarioServicio usuarioServicio;
        private readonly ICitaServicio citaServicio;
        private readonly IExtraServicio extraServicio;

        public ClienteController(IUsuarioServicio usuarioServicio,  ICitaServicio citaServicio, IExtraServicio extraServicio)
        {
            this.usuarioServicio = usuarioServicio;
            this.citaServicio = citaServicio;
            this.extraServicio = extraServicio;
        }

        public IActionResult Index()
        {
            return View();
        }


        //Inicios de sesion

        [HttpGet]

        public IActionResult LoginCliente() { 
            return View();
        }
        [HttpPost]
        public IActionResult LoginCliente(LoginDTO login)
        {
            //Valido Credenciales
            if (string.IsNullOrEmpty(login.Password)) {
                throw new Exception("Verifique ingresar la contraseña.");
            }
            if (string.IsNullOrEmpty(login.NombreUsuario)) { 
                throw new Exception("Verifique ingresar el usuario.");
            }
            //Consulto 
            Cliente UsuarioLogueado = usuarioServicio.IniciarSesionCliente(login);
            HttpContext.Session.SetInt32("ClienteId", UsuarioLogueado.Id);
            //Redirecciono
            TempDataMensaje.SetMensaje(this, "Se inicio sesion correctamente exitoso", "success");
            return RedirectToAction("PanelControlCliente", "Cliente");


        }
        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegistrarUsuario(ClienteDTO cliente)
        {
            //Valido datos ingresados
            if(!ModelState.IsValid)
                return View();
            if(cliente.Password != cliente.ConfPass)
            {
                throw new Exception("La confirmacion no coincide.");
            }
            //Mando a Servicio
            usuarioServicio.RegistrarCliente(cliente);
            return RedirectToAction("Index", "Home");
        }
        //Panel de control cliente
        [HttpGet]
        public IActionResult PanelControlCliente()
        {
            return View();
        }
        //Enviar Cita
        [HttpGet]
        public IActionResult GenerarConsultaCita()
        {
            //Cargo las especialidades y los establecimientos
            ViewBag.Especialidades = extraServicio.ObtenerEspecialidades(); 
            ViewBag.Establecimientos = extraServicio.ObtenerEstablecimientos(); 
            return View(new CitaDTO());
        }
        [HttpPost]
        public IActionResult GenerarConsultaCita(CitaDTO dto)
        {
            int clienteId = (int)HttpContext.Session.GetInt32("ClienteId");
            dto.ClienteId = clienteId;
            //Valido por modelo del dto
            if (!ModelState.IsValid)
                return View(dto);
            //Llamo al repo
            citaServicio.GenerarNuevaCita(dto);

            return RedirectToAction("CitasEnEspera");

        }
        //Listar citas EnEspera de ser aceptadas
        [HttpGet]
        public IActionResult CitasEnEspera()
        {
            int? clienteId = HttpContext.Session.GetInt32("ClienteId");
            var citas = citaServicio.BuscarPorClienteYEstado(clienteId.Value, EstadoCita.EnEspera); // Este método lo implementás en el servicio
            return View(citas); // Pasa la lista de citas a la vista
        }
    }
}
