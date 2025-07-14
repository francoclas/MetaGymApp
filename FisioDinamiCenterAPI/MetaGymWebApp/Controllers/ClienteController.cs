using System.Text.Json;
using LogicaApp.DTOS;
using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using Microsoft.AspNetCore.Mvc;
using static LogicaNegocio.Interfaces.DTOS.EstablecimientoDTO;

namespace MetaGymWebApp.Controllers
{
    [AutorizacionRol("Admin", "Cliente")]
    public class ClienteController : Controller
    {
        private readonly IUsuarioServicio usuarioServicio;
        private readonly ICitaServicio citaServicio;
        private readonly IExtraServicio extraServicio;
        private readonly IRutinaServicio rutinaServicio;

        public ClienteController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IRutinaServicio rutinaServicio)
        {
            this.usuarioServicio = usuarioServicio;
            this.citaServicio = citaServicio;
            this.extraServicio = extraServicio;
            this.rutinaServicio = rutinaServicio;
        }

        public IActionResult Index()
        {
            return View();
        }


        //Inicios de sesion

        [HttpGet]

        public IActionResult LoginCliente()
        {
            return View();
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

            var especialidades = extraServicio.ObtenerEspecialidades();
            var establecimientos = extraServicio.ObtenerEstablecimientos();
            //Mapeo a DTOs para vista
            var establecimientosDTO = establecimientos.Select(e => new EstablecimientoPreviewDTO
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Direccion = e.Direccion,
                UrlMedia = e.Media?.FirstOrDefault()?.Url
            }).ToList();
            ViewBag.Especialidades = especialidades;
            ViewBag.Establecimientos = establecimientos;
            //Serializo para poder mostrar imagenes desde la vista
            ViewBag.EstablecimientosJson = JsonSerializer.Serialize(establecimientosDTO, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return View(new CitaDTO());
        }
        [HttpPost]
        public IActionResult GenerarConsultaCita(CitaDTO dto)
        {
            try
            {
                int clienteId = GestionSesion.ObtenerUsuarioId(this.HttpContext);
                dto.ClienteId = clienteId;
                //Valido informcion
                if (dto.ClienteId == 0) throw new Exception("Vuelva a iniciar sesion.");
                if (string.IsNullOrEmpty(dto.Descripcion)) throw new Exception("La descripcion no puede estar vacia.");
                if (dto.FechaAsistencia < DateTime.Today) throw new Exception("La fecha deseada debe ser posterior a hoy.");
                //Llamo al repo
                citaServicio.GenerarNuevaCita(dto);
                return RedirectToAction("MisCitas");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }
            //Obtengo id del cliente logueado


        }
        //Seccion de citas del cliente
        [HttpGet]
        public IActionResult MisCitas()
        {
            //Obtengo ID y Citas
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var citas = citaServicio.SolicitarHistorialCliente(clienteId);

            //Mapeo a DTO
            var dtoList = citas.Select(c => new CitaDTO
            {
                CitaId = c.Id,
                ClienteId = c.ClienteId,
                Cliente = c.Cliente,
                EspecialidadId = c.EspecialidadId,
                Especialidad = c.Especialidad,
                Estado = c.Estado,
                EstablecimientoId = c.EstablecimientoId,
                Establecimiento = c.Establecimiento,
                Descripcion = c.Descripcion,
                FechaAsistencia = c.FechaAsistencia ?? DateTime.MinValue,
                FechaCreacion = c.FechaCreacion,
                ProfesionalId = c.ProfesionalId,
                Conclusion = c.Conclusion
            }).ToList();
            //Devuelvo
            return View(dtoList);
        }
        //Ir a los detalles de la cita
        [HttpGet]
        public IActionResult VerDetalleCita(int id)
        {
            var cita = citaServicio.ObtenerPorId(id);

            if (cita == null || cita.ClienteId != GestionSesion.ObtenerUsuarioId(HttpContext))
            {
                TempDataMensaje.SetMensaje(this, "No tenés permisos para ver esta cita.", "Error");
                return RedirectToAction("MisCitas");
            }

            var dto = new CitaDTO
            {
                CitaId = cita.Id,
                ClienteId = cita.ClienteId,
                Cliente = cita.Cliente,
                EspecialidadId = cita.EspecialidadId,
                Especialidad = cita.Especialidad,
                EstablecimientoId = cita.EstablecimientoId,
                Establecimiento = cita.Establecimiento,
                Descripcion = cita.Descripcion,
                FechaAsistencia = cita.FechaAsistencia ?? DateTime.MinValue,
                FechaCreacion = cita.FechaCreacion,
                ProfesionalId = cita.ProfesionalId,
                Conclusion = cita.Conclusion,
                Estado = cita.Estado
            };

            return View("DetalleCita", dto);
        }
        //Rutinas

        [HttpGet]
        public IActionResult MisRutinas()
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var asignaciones = rutinaServicio.ObtenerRutinasAsignadasCliente(clienteId);

            return View(asignaciones);
        }
        [HttpGet]
        public IActionResult DetallesRutinaAsignada(int id)
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var dto = rutinaServicio.ObtenerDetalleRutinaAsignadaDTO(id, clienteId);

            if (dto == null)
            {
                TempData["Mensaje"] = "No tenés acceso a esta rutina.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisRutinas");
            }

            return View(dto);
        }
        //Sesiones de entrenamiento
        [HttpGet]
        public IActionResult SesionEntrenada(int id)
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var sesion = rutinaServicio.ObtenerSesionPorId(id);

            // Cargar datos necesarios para vista (Rutina y Ejercicios completos)
            var rutina = rutinaServicio.ObtenerRutinaPorId(sesion.RutinaAsignadaId);

            foreach (var er in sesion.EjerciciosRealizados)
            {
                er.Ejercicio = rutina.Ejercicios
                    .FirstOrDefault(re => re.EjercicioId == er.EjercicioId)?.Ejercicio;
            }

            ViewBag.RutinaNombre = rutina.NombreRutina;
            return View(sesion);
        }
        [HttpGet]
        public IActionResult HistoricoSesionesEntrenamiento()
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var sesiones = rutinaServicio.ObtenerHistorialCliente(clienteId);
            return View(sesiones);
        }

        //Ver detalles de ejercicio
        [HttpGet]
        public IActionResult InformacionEjercicio(int id)
        {
            var dto = rutinaServicio.ObtenerEjercicioDTOId(id);
            if (dto == null)
            {
                TempData["Mensaje"] = "El ejercicio no fue encontrado.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisRutinas");
            }
            return View(dto);
        }
    }
}
