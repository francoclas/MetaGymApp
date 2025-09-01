using System.Text.Json;
using LogicaApp.DTOS;
using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using static LogicaNegocio.Interfaces.DTOS.EstablecimientoDTO;

namespace MetaGymWebApp.Controllers
{
    [AutorizacionRol("Admin", "Cliente","Profesional")]
    public class ClienteController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly ICitaServicio _citaServicio;
        private readonly IExtraServicio _extraServicio;
        private readonly IRutinaServicio _rutinaServicio;

        public ClienteController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IRutinaServicio rutinaServicio)
        {
            this._usuarioServicio = usuarioServicio;
            this._citaServicio = citaServicio;
            this._extraServicio = extraServicio;
            this._rutinaServicio = rutinaServicio;
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
        // GET
        [HttpGet]
        public IActionResult GenerarConsultaCita()
        {
            var especialidades = _extraServicio.ObtenerEspecialidadesDTO(); // devuelve List<EspecialidadDTO>
            var establecimientos = _extraServicio.ObtenerEstablecimientosDTO(); // List<EstablecimientoDTO>
            var tiposAtencion = _extraServicio.ObtenerTiposAtencionDTO(); // List<TipoAtencionDTO>

            var modelo = new GenerarCitaModelo
            {
                Cita = new CitaDTO(),
                Especialidades = especialidades,
                Establecimientos = establecimientos,
                TiposAtencion = tiposAtencion
            };

            return View(modelo);
        }

        // POST
        [HttpPost]
        public IActionResult GenerarConsultaCita(GenerarCitaModelo vm)
        {
            try
            {
                int clienteId = GestionSesion.ObtenerUsuarioId(this.HttpContext);
                vm.Cita.ClienteId = clienteId;

                if (clienteId == 0) throw new Exception("Vuelva a iniciar sesión.");
                if (string.IsNullOrEmpty(vm.Cita.Descripcion)) throw new Exception("La descripción no puede estar vacía.");
                if (vm.Cita.FechaAsistencia < DateTime.Today) throw new Exception("La fecha deseada debe ser posterior a hoy.");
                if (vm.Cita.TipoAtencionId == 0) throw new Exception("Debe seleccionar un tipo de atención.");

                _citaServicio.GenerarNuevaCita(vm.Cita);
                return RedirectToAction("MisCitas");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";

                // Recargo el modelo para la vista
                vm.Especialidades = _extraServicio.ObtenerEspecialidadesDTO();
                vm.Establecimientos = _extraServicio.ObtenerEstablecimientosDTO();
                vm.TiposAtencion = _extraServicio.ObtenerTiposAtencionDTO();

                return View(vm);
            }
        }
        //Seccion de citas del cliente
        [HttpGet]
        public IActionResult MisCitas()
        {
            //Obtengo ID y Citas
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var citas = _citaServicio.SolicitarHistorialCliente(clienteId);

            //Mapeo a DTO
            var dtoList = citas.Select(c => new CitaDTO
            {
                CitaId = c.CitaId,
                ClienteId = c.ClienteId,
                Cliente = c.Cliente,
                EspecialidadId = c.EspecialidadId,
                Especialidad = c.Especialidad,
                Estado = c.Estado,
                EstablecimientoId = c.EstablecimientoId,
                Establecimiento = c.Establecimiento,
                Descripcion = c.Descripcion,
                FechaAsistencia = c.FechaAsistencia,
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
            var cita = _citaServicio.ObtenerPorId(id);

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
                Conclusion = cita.Conclusion,
                Estado = cita.Estado
            };
            if (cita.TipoAtencion != null)
            {
                dto.TipoAtencion = cita.TipoAtencion;
            }
            else
            {
                dto.TipoAtencion = new TipoAtencion { Nombre = "-" };
            }
            if (cita.Profesional != null)
            {
                dto.NombreProfesional = cita.Profesional.NombreCompleto ?? "Aun no asignado";
                dto.TelefonoProfesional = cita.Profesional.Telefono ?? "-";
                dto.CorreoProfesional = cita.Profesional.Correo ?? "-";
                dto.ProfesionalId = cita.ProfesionalId;
            }
            return View("DetalleCita", dto);
        }
        //Rutinas

        [HttpGet]
        public IActionResult MisRutinas()
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            //Obtengo rutinas del cliente
            List<RutinaAsignada> asignaciones = _rutinaServicio.ObtenerRutinasAsignadasCliente(clienteId);
            return View(asignaciones);
        }
        [HttpGet]
        public IActionResult DetallesRutinaAsignada(int id)
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            //OBtengo rutina del repo
            var dto = _rutinaServicio.ObtenerDetalleRutinaAsignadaDTO(id, clienteId);
            //Devuelvo a menu si no existe
            if (dto == null)
            {
                TempData["Mensaje"] = "No tenés acceso a esta rutina.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisRutinas");
            }

            return View(dto);
        }
        [HttpGet]
        public IActionResult SesionEntrenada(int id)
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            SesionRutina sesion = _rutinaServicio.ObtenerSesionPorId(id);

            Rutina rutina = sesion.RutinaAsignada != null
                ? _rutinaServicio.ObtenerRutinaPorId(sesion.RutinaAsignada.RutinaId)
                : null;

            if (rutina != null)
            {
                foreach (var er in sesion.EjerciciosRealizados)
                {
                    er.Ejercicio = rutina.Ejercicios
                        .FirstOrDefault(re => re.Ejercicio.Id == er.EjercicioId)?.Ejercicio;
                }
            }

            var dto = new SesionEntrenadaDTO
            {
                NombreRutina = rutina != null
                    ? rutina.NombreRutina
                    : sesion.NombreRutinaHistorial, // snapshot
                FechaRealizada = sesion.FechaRealizada,
                DuracionMin = sesion.DuracionMin,
                Ejercicios = sesion.EjerciciosRealizados.Select(er => new EjercicioRealizadoDTO
                {
                    Nombre = er.NombreHistorial,
                    Tipo = er.TipoHistorial,
                    GrupoMuscular = er.GrupoMuscularHistorial,
                    SeRealizo =er.SeRealizo,
                    ImagenURL = rutina != null
                        ? er.Ejercicio?.Medias.FirstOrDefault()?.Url
                        : er.ImagenUrlHistorial, // snapshot

                    Series = er.Series.Select(s => new SerieDTO
                    {
                        Repeticiones = s.Repeticiones,
                        PesoUtilizado = s.PesoUtilizado
                    }).ToList(),

                    Mediciones = er.ValoresMediciones.Select(vm => new MedicionDTO
                    {
                        Nombre = vm.Medicion?.Nombre,  // si es snapshot, puede ser null
                        Unidad = vm.Medicion?.Unidad,
                        Valor = vm.Valor
                    }).ToList()
                }).ToList()
            };

            return View(dto);
        }


        [HttpGet]
        public IActionResult HistoricoSesionesEntrenamiento()
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);

            var sesiones = _rutinaServicio.ObtenerHistorialClienteDTO(clienteId);

            var modeloOrdenado = sesiones
                .OrderByDescending(s => s.FechaRealizada)
                .ToList();

            ViewBag.UltimaSesion = modeloOrdenado.FirstOrDefault();
            ViewBag.PromedioDuracion = modeloOrdenado
                .Where(s => s.DuracionMin.HasValue)
                .Select(s => s.DuracionMin.Value)
                .DefaultIfEmpty(0)
                .Average();

            return View(modeloOrdenado);
        }
        //Ver detalles de ejercicio
        [HttpGet]
        public IActionResult InformacionEjercicio(int id)
        {
            var dto = _rutinaServicio.ObtenerEjercicioDTOId(id);
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
