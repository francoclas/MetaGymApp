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
    // Funcionalidad del lado del cliente (citas, rutinas, sesiones, info de ejercicios)
    [AutorizacionRol("Admin", "Cliente", "Profesional")]
    public class ClienteController : Controller
    {
        // Servicios que usa el cliente
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly ICitaServicio _citaServicio;
        private readonly IExtraServicio _extraServicio;
        private readonly IRutinaServicio _rutinaServicio;

        // Inyección de dependencias
        public ClienteController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IRutinaServicio rutinaServicio)
        {
            this._usuarioServicio = usuarioServicio;
            this._citaServicio = citaServicio;
            this._extraServicio = extraServicio;
            this._rutinaServicio = rutinaServicio;
        }

        // Vista base
        public IActionResult Index()
        {
            return View();
        }

        // =======================
        // Autenticación (pantalla específica de cliente)
        // =======================

        [HttpGet]
        public IActionResult LoginCliente()
        {
            return View();
        }

        // =======================
        // Panel general del cliente
        // =======================
        [HttpGet]
        public IActionResult PanelControlCliente()
        {
            return View();
        }

        // =======================
        // Solicitud de cita 
        // =======================

        //Muestra formulario con listas necesarias
        [HttpGet]
        public IActionResult GenerarConsultaCita()
        {
            // Cargo listas para dropdowns
            var especialidades = _extraServicio.ObtenerEspecialidadesDTO();
            var establecimientos = _extraServicio.ObtenerEstablecimientosDTO();
            var tiposAtencion = _extraServicio.ObtenerTiposAtencionDTO();

            // Modelo para la vista
            var modelo = new GenerarCitaModelo
            {
                Cita = new CitaDTO(),
                Especialidades = especialidades,
                Establecimientos = establecimientos,
                TiposAtencion = tiposAtencion
            };

            return View(modelo);
        }

        // Valida y registra la solicitud de cita
        [HttpPost]
        public IActionResult GenerarConsultaCita(GenerarCitaModelo vm)
        {
            try
            {
                // Identifico al cliente logueado
                int clienteId = GestionSesion.ObtenerUsuarioId(this.HttpContext);
                vm.Cita.ClienteId = clienteId;

                // Validaciones mínimas
                if (clienteId == 0) throw new Exception("Vuelva a iniciar sesión.");
                if (string.IsNullOrEmpty(vm.Cita.Descripcion)) throw new Exception("La descripción no puede estar vacía.");
                if (vm.Cita.FechaAsistencia < DateTime.Today) throw new Exception("La fecha deseada debe ser posterior a hoy.");
                if (vm.Cita.TipoAtencionId == 0) throw new Exception("Debe seleccionar un tipo de atención.");

                // Registro la cita en el sistema
                _citaServicio.GenerarNuevaCita(vm.Cita);
                TempData["Mensaje"] = "Se genero solicitud de citas, se notificaran los avances.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("MisCitas");
            }
            catch (Exception e)
            {
                // Informo y recargo combos
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";

                vm.Especialidades = _extraServicio.ObtenerEspecialidadesDTO();
                vm.Establecimientos = _extraServicio.ObtenerEstablecimientosDTO();
                vm.TiposAtencion = _extraServicio.ObtenerTiposAtencionDTO();

                return View(vm);
            }
        }

        // =======================
        // Citas del cliente
        // =======================

        // Listado de citas propias
        [HttpGet]
        public IActionResult MisCitas()
        {
            // Tomo el id del cliente y traigo su historial
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var citas = _citaServicio.SolicitarHistorialCliente(clienteId);

            // Mapeo a DTO que consume la vista
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

            return View(dtoList);
        }

        // Detalle puntual de una cita (verificación de propiedad)
        [HttpGet]
        public IActionResult VerDetalleCita(int id)
        {
            var cita = _citaServicio.ObtenerPorId(id);
            if (cita == null)
            {
                TempData["Mensaje"] = "La cita se elimino o no existe.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisCitas");
            }
            // Seguridad: solo el dueño puede ver la cita
            if ( cita.ClienteId != GestionSesion.ObtenerUsuarioId(HttpContext))
            {
                TempData["Mensaje"] = "No tenés permisos para ver esta cita.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisCitas");
            }

            // Armo DTO detallado
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

            // Tipo de atención (si no hay, muestro "-")
            if (cita.TipoAtencion != null)
            {
                dto.TipoAtencion = cita.TipoAtencion;
            }
            else
            {
                dto.TipoAtencion = new TipoAtencion { Nombre = "-" };
            }

            // Datos del profesional (si ya está asignado)
            if (cita.Profesional != null)
            {
                dto.NombreProfesional = cita.Profesional.NombreCompleto ?? "Aun no asignado";
                dto.TelefonoProfesional = cita.Profesional.Telefono ?? "-";
                dto.CorreoProfesional = cita.Profesional.Correo ?? "-";
                dto.ProfesionalId = cita.ProfesionalId;
            }
            else
            {
                dto.NombreProfesional = "Pendiente de revision";
                dto.TelefonoProfesional =  "-";
                dto.CorreoProfesional = "-";
            }

                return View("DetalleCita", dto);
        }

        // =======================
        // Rutinas asignadas al cliente
        // =======================

        [HttpGet]
        public IActionResult MisRutinas()
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            // Traigo asignaciones de rutinas
            List<RutinaAsignada> asignaciones = _rutinaServicio.ObtenerRutinasAsignadasCliente(clienteId);
            return View(asignaciones);
        }

        [HttpGet]
        public IActionResult DetallesRutinaAsignada(int id)
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
            // Pido detalle y valido acceso
            var dto = _rutinaServicio.ObtenerDetalleRutinaAsignadaDTO(id, clienteId);
            if (dto == null)
            {
                TempData["Mensaje"] = "No tenés acceso a esta rutina.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisRutinas");
            }

            return View(dto);
        }

        // =======================
        // Sesión entrenada (detalle histórico)
        // =======================

        [HttpGet]
        public IActionResult SesionEntrenada(int id)
        {
            try
            {
                int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);
                SesionRutina sesion = _rutinaServicio.ObtenerSesionPorId(id);
                if (sesion == null)
                    throw new Exception("No se encontro sesion o se elimino.");
                if(sesion.ClienteId != clienteId)
                    throw new Exception("No tiene permisos para ver esta sesion.");

                // Si la rutina original existe, la uso para completar datos (si no, uso snapshot)
                Rutina rutina = sesion.RutinaAsignada != null
                    ? _rutinaServicio.ObtenerRutinaPorId(sesion.RutinaAsignada.RutinaId)
                    : null;

                if (rutina != null)
                {
                    // Enlazo cada ejercicio realizado con el ejercicio actual de la rutina (si coincide)
                    foreach (var er in sesion.EjerciciosRealizados)
                    {
                        er.Ejercicio = rutina.Ejercicios
                            .FirstOrDefault(re => re.Ejercicio.Id == er.EjercicioId)?.Ejercicio;
                    }
                }

                // Armo DTO para la vista
                var dto = new SesionEntrenadaDTO
                {
                    NombreRutina = rutina != null
                        ? rutina.NombreRutina
                        : sesion.NombreRutinaHistorial, // snapshot si la rutina cambió
                    FechaRealizada = sesion.FechaRealizada,
                    DuracionMin = sesion.DuracionMin,
                    Ejercicios = sesion.EjerciciosRealizados.Select(er => new EjercicioRealizadoDTO
                    {
                        Nombre = er.NombreHistorial,
                        Tipo = er.TipoHistorial,
                        GrupoMuscular = er.GrupoMuscularHistorial,
                        SeRealizo = er.SeRealizo,
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
                            Nombre = vm.Medicion?.Nombre,  // si es snapshot, puede venir null
                            Unidad = vm.Medicion?.Unidad,
                            Valor = vm.Valor
                        }).ToList()
                    }).ToList()
                };

                return View(dto);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("HistoricoSesionesEntrenamiento");
            }
        }

        // Historial completo de sesiones entrenadas
        [HttpGet]
        public IActionResult HistoricoSesionesEntrenamiento()
        {
            int clienteId = GestionSesion.ObtenerUsuarioId(HttpContext);

            var sesiones = _rutinaServicio.ObtenerHistorialClienteDTO(clienteId);

            // Ordeno desc, calculo agregados para la vista
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

        // =======================
        // Info de ejercicio
        // =======================

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