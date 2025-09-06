using System.ComponentModel.Design;
using LogicaApp.DTOS;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MetaGymWebApp.Controllers
{
    // Controlador de funcionalidades del profesional (citas, rutinas, ejercicios, publicaciones, calendario)
    [AutorizacionRol("Profesional")]
    public class ProfesionalController : Controller
    {
        // Servicios que usa el profesional
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IProfesionalServicio _profesionalServicio;
        private readonly ICitaServicio _citaServicio;
        private readonly IExtraServicio _extraServicio;
        private readonly IRutinaServicio _rutinaServicio;
        private readonly IMediaServicio _mediaServicio;
        private readonly IClienteServicio _clienteServicio;
        private readonly IPublicacionServicio _publicacionServicio;
        private readonly INotificacionServicio _notificacionServicio;

        // Inyección de dependencias
        public ProfesionalController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IProfesionalServicio proservicio, IRutinaServicio rutina, IMediaServicio mediaServicio, IClienteServicio clienteServicio, IPublicacionServicio publicacionServicio, INotificacionServicio notificacion)
        {
            this._usuarioServicio = usuarioServicio;
            this._citaServicio = citaServicio;
            this._extraServicio = extraServicio;
            this._profesionalServicio = proservicio;
            this._rutinaServicio = rutina;
            this._mediaServicio = mediaServicio;
            this._clienteServicio = clienteServicio;
            this._publicacionServicio = publicacionServicio;
            this._notificacionServicio = notificacion;
        }

        // =======================
        // Citas: Bandeja de solicitudes pendientes
        // =======================

        [HttpGet]
        public IActionResult VerSolicitudCitas()
        {
            // Traigo citas "EnEspera" para que el profesional las acepte/rechace
            List<Cita> salida = _citaServicio.BuscarPorEstado(EstadoCita.EnEspera);
            return View(salida);
        }

        // =======================
        // Historias clínicas / búsqueda de clientes
        // =======================

        [HttpGet]
        public IActionResult BuscarClientesCita()
        {
            // Listado liviano de clientes para seleccionar y ver detalle
            var listaClientes = _clienteServicio.ObtenerTodosDTO()
                .Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    NombreCompleto = c.NombreCompleto,
                    Ci = c.Ci,
                    Correo = c.Correo,
                    Telefono = c.Telefono
                }).ToList();

            return View(listaClientes);
        }

        // =======================
        // Gestión de una cita puntual
        // =======================

        [HttpGet]
        public IActionResult RevisarCita(int id)
        {
            // Cargo datos mínimos para mostrar la cita antes de aceptarla
            Cita Cita = _citaServicio.ObtenerPorId(id);
            return View(new CitaDTO
            {
                CitaId = Cita.Id,
                Cliente = Cita.Cliente,
                Establecimiento = Cita.Establecimiento,
                Especialidad = Cita.Especialidad,
                Descripcion = Cita.Descripcion,
                FechaAsistencia = (DateTime)Cita.FechaAsistencia,
            });
        }

        [HttpPost]
        public IActionResult RevisarCita(CitaDTO citaDTO)
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(this.HttpContext);

                // Recupero la cita original
                Cita cita = _citaServicio.ObtenerPorId(citaDTO.CitaId);

                // Evito doble gestión
                if (cita.Estado != EstadoCita.EnEspera)
                    throw new Exception("La cita ya fue gestionada.");

                // Me asigno la cita y la acepto
                cita.ProfesionalId = profesionalId;
                cita.Estado = EstadoCita.Aceptada;

                // Validación de dominio (horarios, etc.)
                cita.Validar();

                // Persisto
                _citaServicio.ActualizarEntidad(cita);

                TempData["Mensaje"] = "Cita aceptada correctamente.";
                return RedirectToAction("GestionCitas");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View(citaDTO);
            }
        }

        // Vista general de citas: en espera / próximas / atendidas
        public IActionResult GestionCitas()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

            try
            {
                // Si el profesional no tiene tipos de atención asociados, muestro aviso y nada más
                List<int> tiene = _profesionalServicio.ObtenerTiposAtencionProfesional(profesionalId);
                if (!tiene.Any())
                {
                    GestionCitasModelo salida = new GestionCitasModelo { TieneEspecialidades = false };
                    return View(salida);
                }
                else
                {
                    // Citas en espera según los tipos que atiende
                    var citasEnEspera = _citaServicio
                        .BuscarSolicitudesSegunTiposAtencion(_profesionalServicio.ObtenerTiposAtencionProfesional(profesionalId));

                    // Próximas (asignadas al profesional y aún por atender)
                    var citasProximas = _citaServicio.SolicitarProximasProfesional(profesionalId);

                    // Historial filtrado a finalizadas (para no mezclar estados)
                    var citasFinalizadas = _citaServicio
                        .SolicitarHistorialProfesional(profesionalId)
                        .Where(c => c.Estado == EstadoCita.Finalizada)
                        .ToList();

                    // Mapeo a DTOs para la vista
                    GestionCitasModelo modelo = new GestionCitasModelo
                    {
                        CitasEnEspera = citasEnEspera.Select(c => new CitaDTO
                        {
                            CitaId = c.Id,
                            Cliente = c.Cliente,
                            Especialidad = c.Especialidad,
                            Establecimiento = c.Establecimiento,
                            Descripcion = c.Descripcion,
                            FechaAsistencia = c.FechaAsistencia ?? DateTime.MinValue,
                            FechaCreacion = c.FechaCreacion,
                            ProfesionalId = c.ProfesionalId,
                            Conclusion = c.Conclusion
                        }).ToList(),

                        CitasProximasDeProfesional = citasProximas.Select(c => new CitaDTO
                        {
                            CitaId = c.Id,
                            Cliente = c.Cliente,
                            Especialidad = c.Especialidad,
                            Establecimiento = c.Establecimiento,
                            Descripcion = c.Descripcion,
                            FechaAsistencia = c.FechaAsistencia ?? DateTime.MinValue,
                            FechaCreacion = c.FechaCreacion,
                            ProfesionalId = c.ProfesionalId
                        }).ToList(),

                        CitasAtendidasProfesional = citasFinalizadas.Select(c => new CitaDTO
                        {
                            CitaId = c.Id,
                            Cliente = c.Cliente,
                            Especialidad = c.Especialidad,
                            Establecimiento = c.Establecimiento,
                            Descripcion = c.Descripcion,
                            FechaAsistencia = c.FechaAsistencia ?? DateTime.MinValue,
                            FechaCreacion = c.FechaCreacion,
                            ProfesionalId = c.ProfesionalId,
                            Conclusion = c.Conclusion
                        }).ToList(),
                        TieneEspecialidades = true
                    };

                    return View(modelo);
                }
            }
            catch (Exception e)
            {

                TempData["Mensaje"] = "No tenes asignada la rutina a editar";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionCitas");
            }
        }

        // =======================
        // Editar cita
        // =======================

        [HttpGet]
        public IActionResult EditarCita(int id)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var cita = _citaServicio.ObtenerPorId(id);

            // Seguridad: la cita debe ser del profesional logueado
            if (cita.ProfesionalId != ProfesionalId)
            {
                TempData["Mensaje"] = "No tenes asignada la rutina a editar";
                TempData["TipoMensaje"] = "danger";
                return View("GestionCitas");
            }

            // Armo DTO para la vista
            var dto = new CitaDTO
            {
                CitaId = cita.Id,
                FechaAsistencia = cita.FechaAsistencia ?? DateTime.Now,
                EstablecimientoId = cita.EstablecimientoId,
                ClienteId = cita.ClienteId,
                ProfesionalId = cita.ProfesionalId,
                EspecialidadId = cita.EspecialidadId,
                Descripcion = cita.Descripcion,
                TipoAtencionId = cita.TipoAtencionId
            };

            // Combo con tipos de atención disponibles para el profesional
            ViewBag.TiposAtencion = new SelectList(_extraServicio.ObtenerTiposAtencionPorProfesional(ProfesionalId), "Id", "Nombre", dto.TipoAtencionId);
            return View(dto);
        }

        [HttpGet]
        public IActionResult EditarCitaParcial(int citaId)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            Cita cita = _citaServicio.ObtenerPorId(citaId);

            if (cita.ProfesionalId != ProfesionalId)
            {
                TempData["Mensaje"] = "No tenes asignada la rutina a editar";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("Parciales/_EditarCitaParcial");
            }

            var dto = new CitaDTO
            {
                CitaId = cita.Id,
                FechaAsistencia = cita.FechaAsistencia ?? DateTime.Now,
                EstablecimientoId = cita.EstablecimientoId,
                ClienteId = cita.ClienteId,
                Cliente = cita.Cliente,
                Establecimiento = cita.Establecimiento,
                Especialidad = cita.Especialidad,
                ProfesionalId = cita.ProfesionalId,
                EspecialidadId = cita.EspecialidadId,
                Descripcion = cita.Descripcion,
                TipoAtencionId = cita.TipoAtencionId
            };

            ViewBag.TiposAtencion = new SelectList(_extraServicio.ObtenerTiposAtencionPorProfesional(ProfesionalId), "Id", "Nombre", dto.TipoAtencionId);
            return PartialView("Parciales/_EditarCitaParcial", dto);
        }

        [HttpPost]
        public IActionResult EditarCita(CitaDTO citaDTO)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            try
            {
                Cita cita = _citaServicio.ObtenerPorId(citaDTO.CitaId);
                if (cita == null)
                    throw new Exception("No se encontró la cita.");

                // Seguridad
                if (cita.ProfesionalId != ProfesionalId)
                {
                    TempData["Mensaje"] = "No tenes asignada la rutina a editar";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("GestionCitas");
                }

                // Validaciones de estado (si cierra/cancela/no asiste/rechaza debe tener conclusión)
                if (citaDTO.Estado == EstadoCita.Finalizada ||
                    citaDTO.Estado == EstadoCita.Cancelada ||
                    citaDTO.Estado == EstadoCita.NoAsistio ||
                    citaDTO.Estado == EstadoCita.Rechazada)
                {
                    if (string.IsNullOrWhiteSpace(citaDTO.Conclusion))
                        throw new Exception("Debe ingresar una conclusión para finalizar o cancelar la cita.");

                    cita.Conclusion = citaDTO.Conclusion;
                    cita.Estado = citaDTO.Estado;

                    // Si finaliza, registro fecha de finalización
                    if (citaDTO.Estado == EstadoCita.Finalizada)
                    {
                        cita.FechaFinalizacion = DateTime.Now;
                    }
                }
                else
                {
                    // Cambios de flujo sin cierre (ej. Aceptada)
                    cita.Estado = citaDTO.Estado;
                }

                // Fecha, descripción y tipo de atención (si vino)
                cita.FechaAsistencia = citaDTO.FechaAsistencia;
                cita.Descripcion = citaDTO.Descripcion;
                if (citaDTO.TipoAtencionId.HasValue)
                    cita.TipoAtencionId = citaDTO.TipoAtencionId.Value;

                cita.Validar(); // reglas de dominio
                _citaServicio.ActualizarEntidad(cita);

                TempData["Mensaje"] = "Cita actualizada correctamente.";
                return RedirectToAction("GestionCitas");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";

                // Repoblar combo por si vuelve a la vista con error
                ViewBag.TiposAtencion = new SelectList(_extraServicio.ObtenerTiposAtencionPorProfesional(ProfesionalId), "Id", "Nombre", citaDTO.TipoAtencionId);
                return View(citaDTO);
            }
        }

        [HttpPost]
        public IActionResult EditarCitaCalendario(CitaDTO citaDTO)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            try
            {
                Cita cita = _citaServicio.ObtenerPorId(citaDTO.CitaId);

                if (cita == null)
                    return Json(new { success = false, message = "No se encontró la cita." });

                if (cita.ProfesionalId != ProfesionalId)
                    return Json(new { success = false, message = "No tenés asignada la cita a editar." });

                if (citaDTO.Estado == EstadoCita.Finalizada ||
                    citaDTO.Estado == EstadoCita.Cancelada ||
                    citaDTO.Estado == EstadoCita.NoAsistio ||
                    citaDTO.Estado == EstadoCita.Rechazada)
                {
                    if (string.IsNullOrWhiteSpace(citaDTO.Conclusion))
                        return Json(new { success = false, message = "Debe ingresar una conclusión para finalizar o cancelar la cita." });

                    cita.Conclusion = citaDTO.Conclusion;
                    cita.Estado = citaDTO.Estado;
                    if (citaDTO.Estado == EstadoCita.Finalizada)
                        cita.FechaFinalizacion = DateTime.Now;
                }
                else
                {
                    cita.Estado = citaDTO.Estado;
                }

                // Actualizaciones base
                cita.FechaAsistencia = citaDTO.FechaAsistencia;
                cita.Descripcion = citaDTO.Descripcion;
                if (citaDTO.TipoAtencionId.HasValue)
                    cita.TipoAtencionId = citaDTO.TipoAtencionId.Value;

                cita.Validar();
                _citaServicio.ActualizarEntidad(cita);

                TempData["Mensaje"] = "Se actualizo cita";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Calendario");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("Calendario");
            }
        }

        [HttpPost]
        public IActionResult ReprogramarCita(int citaId, DateTime nuevaFecha)
        {
            // Fuerzo a Local para evitar problemas de TZ/serialización al comparar con agendas
            nuevaFecha = DateTime.SpecifyKind(nuevaFecha, DateTimeKind.Local);
            try
            {
                Cita cita = _citaServicio.ObtenerPorId(citaId);
                Profesional profesional = _profesionalServicio.ObtenerProfesional(GestionSesion.ObtenerUsuarioId(HttpContext));

                if (cita == null) throw new Exception("No se encontro cita.");
                if (cita.Estado != EstadoCita.Aceptada) throw new Exception("Solo se puede reprogramar una cita Aceptada.");

                // Chequeo que la nueva fecha caiga dentro de una franja laboral activa
                bool dentroFranja = EstaDentroDeFranja(profesional.Agendas, nuevaFecha);
                if (!dentroFranja) throw new Exception("No se puede reprogramar la cita a esa fecha, no hay jornada laboral en esa hora.");

                // Actualizo fecha y guardo
                cita.FechaAsistencia = nuevaFecha;
                _citaServicio.ActualizarEntidad(cita);

                // Notificación al cliente
                _notificacionServicio.NotificacionPersonalizada(cita.ClienteId, "Cliente",
                    new Notificacion
                    {
                        ClienteId = cita.ClienteId,
                        CitaId = cita.Id,
                        Tipo = Enum_TipoNotificacion.Cita,
                        Titulo = "Se reprogramo tu cita!",
                        Mensaje = "Tu cita con " + cita.Profesional.NombreCompleto + " se traslado a la fecha: " + nuevaFecha,
                        Fecha = DateTime.UtcNow,
                        Leido = false
                    });
                return Ok();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return BadRequest();
            }
        }

        // =======================
        // Historial del cliente clínico y sesiones
        // =======================

        [HttpGet]
        public IActionResult HistorialClinicoCliente(int id)
        {
            // Solo citas finalizadas (las que dejan historial clínico)
            List<CitaDTO> historial = _citaServicio.SolicitarHistorialCliente(id)
                .Where(c => c.Estado == EstadoCita.Finalizada)
                .ToList();

            return View(historial);
        }

        [HttpGet]
        public IActionResult HistorialSesionesCliente(int id)
        {
            // Sesiones de entrenamiento ejecutadas por el cliente
            List<SesionEntrenadaDTO> sesiones = _rutinaServicio.ObtenerHistorialClienteDTO(id);
            return View(sesiones);
        }

        // =======================
        // Rutinas
        // =======================

        [HttpGet]
        public IActionResult GestionRutinas()
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                // Listo rutinas del profesional
                List<Rutina> rutinas = _rutinaServicio.ObtenerRutinasProfesional(profesionalId);
                if (rutinas == null) throw new Exception("No se obtuvieron las rutinas, intente iniciando sesion nuevamente.");

                // Mapeo liviano para la grilla
                return View(rutinas.Select(r => new RutinaDTO
                {
                    Id = r.Id,
                    NombreRutina = r.NombreRutina,
                    Tipo = r.Tipo,
                    UltimaModificacion = DateTime.Now
                }).ToList());
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("Inicio", "Publicacion");
            }
        }

        // =======================
        // Ver una cita
        // =======================

        [HttpGet]
        public IActionResult VerCita(int id)
        {
            var cita = _citaServicio.ObtenerPorId(id);
            return View("VerCita", new CitaDTO
            {
                CitaId = cita.Id,
                Cliente = cita.Cliente,
                Establecimiento = cita.Establecimiento,
                Especialidad = cita.Especialidad,
                Estado = cita.Estado,
                Descripcion = cita.Descripcion,
                FechaAsistencia = cita.FechaAsistencia ?? DateTime.Now,
                Conclusion = cita.Conclusion,
                NombreProfesional = cita.Profesional.NombreCompleto ?? "-",
                TelefonoProfesional = cita.Profesional.Telefono ?? "-"
            });
        }

        [HttpGet]
        public IActionResult VerCitaParcial(int citaId)
        {
            var cita = _citaServicio.ObtenerPorId(citaId);
            return PartialView("Parciales/_VistaCitaParcial", new CitaDTO
            {
                CitaId = cita.Id,
                Cliente = cita.Cliente,
                Establecimiento = cita.Establecimiento,
                Especialidad = cita.Especialidad,
                Estado = cita.Estado,
                Descripcion = cita.Descripcion,
                FechaAsistencia = cita.FechaAsistencia ?? DateTime.Now,
                Conclusion = cita.Conclusion
            });
        }

        // =======================
        // Crear cita (vista y parcial)
        // =======================

        [HttpGet]
        public IActionResult CrearCita()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

            // Se preparan combos: clientes, especialidades+tipos y establecimientos
            RegistroCitaModelo modelo = new RegistroCitaModelo
            {
                Clientes = _clienteServicio.ObtenerTodosDTO(),
                // Este ya trae los tipos de atención dentro de cada especialidad
                Especialidades = _profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId),
                Establecimientos = _extraServicio.ObtenerEstablecimientosDTO(),
                Cita = new CitaDTO()
            };

            return View(modelo);
        }

        [HttpGet]
        public IActionResult CrearCitaParcial(DateTime? fecha)
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

            RegistroCitaModelo modelo = new RegistroCitaModelo
            {
                Clientes = _clienteServicio.ObtenerTodosDTO(),
                Especialidades = _profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId),
                Establecimientos = _extraServicio.ObtenerEstablecimientosDTO(),
                Cita = new CitaDTO
                {
                    FechaAsistencia = fecha ?? DateTime.Now
                }
            };

            return PartialView("Parciales/_CrearCitaParcial", modelo);
        }

        [HttpPost]
        public IActionResult CrearCitaParcial(RegistroCitaModelo modelo)
        {
            try
            {
                CitaDTO cita = modelo.Cita;
                cita.ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                Profesional profesional = _profesionalServicio.ObtenerProfesional(GestionSesion.ObtenerUsuarioId(HttpContext));

                // Validaciones mínimas de negocio
                if (cita.ClienteId <= 0)
                    throw new Exception("Debe seleccionar un cliente.");
                if (cita.EspecialidadId <= 0)
                    throw new Exception("Debe seleccionar una especialidad.");
                if (cita.TipoAtencionId <= 0)
                    throw new Exception("Debe seleccionar un tipo de atención.");
                if (cita.EstablecimientoId <= 0)
                    throw new Exception("Debe seleccionar un establecimiento.");
                if (string.IsNullOrWhiteSpace(cita.Descripcion))
                    throw new Exception("Debe ingresar una descripción para la cita.");
                if (cita.FechaAsistencia == default || cita.FechaAsistencia < DateTime.Now)
                    throw new Exception("Debe ingresar una fecha y hora válida y futura.");

                // Chequeo de franja horaria de agenda
                bool dentroFranja = EstaDentroDeFranja(profesional.Agendas, cita.FechaAsistencia);
                if (!dentroFranja)
                    throw new Exception("La fecha seleccionada no está dentro de la franja horaria asignada.");

                _citaServicio.RegistrarCitaPorProfesional(cita);

                TempData["Mensaje"] = "Se creo cita";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Calendario");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("Calendario");
            }
        }

        [HttpPost]
        public IActionResult CrearCita(RegistroCitaModelo modelo)
        {
            try
            {
                CitaDTO cita = modelo.Cita;
                cita.ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                Profesional profesional = _profesionalServicio.ObtenerProfesional(GestionSesion.ObtenerUsuarioId(HttpContext));

                // Validaciones de negocio
                if (cita.ClienteId <= 0)
                    throw new Exception("Debe seleccionar un cliente.");
                if (cita.EspecialidadId <= 0)
                    throw new Exception("Debe seleccionar una especialidad.");
                if (cita.TipoAtencionId <= 0)
                    throw new Exception("Debe seleccionar un tipo de atención.");
                if (cita.EstablecimientoId <= 0)
                    throw new Exception("Debe seleccionar un establecimiento.");
                if (string.IsNullOrWhiteSpace(cita.Descripcion))
                    throw new Exception("Debe ingresar una descripción para la cita.");
                if (cita.FechaAsistencia == default || cita.FechaAsistencia < DateTime.Now)
                    throw new Exception("Debe ingresar una fecha y hora válida y futura.");

                // Verificación de agenda/horario
                bool dentroFranja = EstaDentroDeFranja(profesional.Agendas, cita.FechaAsistencia);
                if (!dentroFranja)
                    throw new Exception("La fecha seleccionada no está dentro de la franja horaria asignada.");

                // Registro en servicio
                _citaServicio.RegistrarCitaPorProfesional(cita);

                TempData["Mensaje"] = "Cita registrada correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionCitas");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";

                // Recargar listas por si se vuelve a la vista con errores
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                modelo.Clientes = _clienteServicio.ObtenerTodosDTO();
                modelo.Especialidades = _profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId);
                modelo.TiposAtencion = _extraServicio.ObtenerTiposAtencionPorProfesionalDTO(profesionalId);
                modelo.Establecimientos = _extraServicio.ObtenerEstablecimientosDTO();

                return View(modelo);
            }
        }

        // =======================
        // Ejercicios
        // =======================

        [HttpGet]
        public IActionResult GestionEjercicios()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            // Todos los ejercicios del sistema
            List<EjercicioDTO> todos = _rutinaServicio.ObtenerTodosEjercicios();

            // Separo los míos de los globales
            GestionEjerciciosModelo modelo = new GestionEjerciciosModelo
            {
                EjerciciosProfesional = todos.Where(e => e.ProfesionalId == profesionalId).ToList(),
                EjerciciosSistema = todos.Where(e => e.ProfesionalId != profesionalId).ToList()
            };

            return View(modelo);
        }

        [HttpGet]
        public IActionResult BuscarHistorialClientes()
        {
            // Helper de búsqueda para elegir cliente y abrir sus historiales
            var listaClientes = _clienteServicio.ObtenerTodosDTO()
                 .Select(c => new ClienteDTO
                 {
                     Id = c.Id,
                     NombreCompleto = c.NombreCompleto,
                     Ci = c.Ci,
                     Correo = c.Correo,
                     Telefono = c.Telefono
                 }).ToList();

            return View(listaClientes);
        }

        [HttpGet]
        public IActionResult DetalleEjercicio(int id)
        {
            try
            {
                // Muestro un ejercicio propio (con permiso de autor)
                EjercicioDTO ejercicio = _rutinaServicio.ObtenerEjercicioDTOId(id);
                if (ejercicio == null) throw new Exception("No se encontro ejercicio o no existe.");
                if (ejercicio.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar este ejercicio, no es el autor.");
                return View(ejercicio);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionEjercicios");
            }
        }

        [HttpGet]
        public IActionResult InformacionEjercicio(int id)
        {
            try
            {
                // Versión solo lectura (info general)
                EjercicioDTO ejercicio = _rutinaServicio.ObtenerEjercicioDTOId(id);
                if (ejercicio == null) throw new Exception("No se encontro ejercicio o no existe.");
                return View(ejercicio);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionEjercicios");
            }
        }

        // Alta de ejercicio (vista + post)
        [HttpGet]
        public IActionResult RegistrarEjercicio()
        {
            return View(new EjercicioDTO());
        }

        [HttpPost]
        public IActionResult RegistrarEjercicio(EjercicioDTO dto, List<IFormFile> archivos)
        {
            // Validaciones mínimas
            try
            {
                if (String.IsNullOrEmpty(dto.Nombre)) throw new Exception("El nombre no puede estar vacio.");
                if (String.IsNullOrEmpty(dto.GrupoMuscular)) throw new Exception("Debe colocar al menos un grupo muscular");
                if (String.IsNullOrEmpty(dto.Tipo)) throw new Exception("Debe colocar un tipo de ejercicio.");
                if (String.IsNullOrEmpty(dto.Instrucciones)) throw new Exception("Debe completar las instrucciones.");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }

            // Instancio entidad para guardar
            Ejercicio ejercicio = new Ejercicio
            {
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                GrupoMuscular = dto.GrupoMuscular,
                Instrucciones = dto.Instrucciones,
                ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext),
                Medias = new List<Media>(),
                Mediciones = dto.Mediciones
            };
            try
            {
                // Persiste y obtiene Id
                _rutinaServicio.GenerarNuevoEjercicio(ejercicio);
                // Si hay archivos, los adjunto
                if (archivos != null && archivos.Count > 0)
                {
                    foreach (var archivo in archivos)
                    {
                        var media = _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Ejercicio, ejercicio.Id);
                        ejercicio.Medias.Add(media);
                    }
                }
                TempData["Mensaje"] = "Se registro el ejercicio: " + ejercicio.Nombre;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionEjercicios");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionEjercicios");
            }
        }

        // Edición de ejercicio
        [HttpGet]
        public IActionResult EditarEjercicio(int id)
        {
            try
            {
                EjercicioDTO dto = _rutinaServicio.ObtenerEjercicioDTOId(id);
                if (dto == null) throw new Exception("No se encontro ejercicio o no existe.");
                if (dto.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar este ejercicio.");
                return View(dto);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionEjercicios");
            }
        }

        [HttpPost]
        public IActionResult EditarEjercicio(EjercicioDTO dto, List<IFormFile> archivos)
        {
            try
            {
                // Traigo la entidad completa
                Ejercicio ejercicio = _rutinaServicio.ObtenerEjercicioId(dto.Id);
                if (ejercicio == null) throw new Exception("No se encontro ejercicio o no existe.");
                if (dto.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar este ejercicio.");

                // Actualizo datos
                ejercicio.Nombre = dto.Nombre;
                ejercicio.Tipo = dto.Tipo;
                ejercicio.GrupoMuscular = dto.GrupoMuscular;
                ejercicio.Instrucciones = dto.Instrucciones;
                ejercicio.Mediciones = dto.Mediciones;

                // Adjuntos nuevos (si hay)
                if (archivos != null && archivos.Count > 0)
                {
                    foreach (var archivo in archivos)
                    {
                        var media = _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Ejercicio, ejercicio.Id);
                        ejercicio.Medias.Add(media);
                    }
                }
                _rutinaServicio.ModificarEjercicio(ejercicio);
                return RedirectToAction("GestionEjercicios");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionEjercicios");
            }
        }

        [HttpPost]
        public IActionResult EliminarEjercicio(int id)
        {
            try
            {
                // Seguridad y existencia
                Ejercicio ejercicio = _rutinaServicio.ObtenerEjercicioId(id);
                if (ejercicio == null)
                    throw new Exception("No se encontró ejercicio o no existe.");
                if (ejercicio.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext))
                    throw new Exception("No tiene permisos para eliminar este ejercicio.");

                bool SeElimino = _rutinaServicio.EliminarEjercicio(id);
                if (!SeElimino)
                    throw new Exception("No se pudo eliminar el ejercicio.");

                TempData["Mensaje"] = "Ejercicio eliminado correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionEjercicios");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionEjercicios");
            }
        }

        // =======================
        // Rutinas: altas/edición/asignación
        // =======================

        [HttpGet]
        public IActionResult RegistrarRutina()
        {
            // Preparo combos: ejercicios propios, de sistema y clientes
            int profesionalId = GestionSesion.ObtenerUsuarioId(this.HttpContext);
            List<EjercicioDTO> todos = _rutinaServicio.ObtenerTodosEjercicios();

            RutinaRegistroDTO modelo = new RutinaRegistroDTO
            {
                MisEjerciciosDisponibles = todos.Where(e => e.ProfesionalId == profesionalId).ToList(),
                EjerciciosDisponiblesSistema = todos.Where(e => e.ProfesionalId != profesionalId).ToList(),
                ClientesDisponibles = _clienteServicio.ObtenerTodosDTO()
            };
            return View(modelo);
        }

        [HttpPost]
        public IActionResult RegistrarRutina(RutinaRegistroDTO dto)
        {
            try
            {
                // Creo la rutina vacía (necesito Id para luego asignar ejercicios)
                var rutina = new Rutina
                {
                    NombreRutina = dto.NombreRutina,
                    Tipo = dto.Tipo,
                    ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext),
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    Ejercicios = new List<RutinaEjercicio>()
                };

                // Persisto para obtener Id
                _rutinaServicio.GenerarNuevaRutina(rutina);

                // Cargo ejercicios seleccionados (patrón de merge en servicio)
                _rutinaServicio.ActualizarEjerciciosRutina(rutina, dto.IdsEjerciciosSeleccionados);

                // Asigno la rutina a cada cliente indicado
                foreach (var clienteId in dto.IdsClientesAsignados)
                {
                    _rutinaServicio.AsignarRutinaACliente(clienteId, rutina.Id);
                }

                TempData["Mensaje"] = "Se registró la rutina correctamente y se asignó a los usuarios seleccionados.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionRutinas");
            }
            catch (Exception)
            {
                TempData["Mensaje"] = "No se logró registrar la rutina, favor de intentar nuevamente más tarde.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionRutinas");
            }
        }

        [HttpGet]
        public IActionResult EditarRutina(int id)
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(this.HttpContext);

                // Traigo la rutina + chequeo permisos
                Rutina rutina = _rutinaServicio.ObtenerRutinaPorId(id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para editar esta rutina.");

                // ids de ejercicios ya asignados
                var idsSeleccionados = rutina.Ejercicios.Select(e => e.EjercicioId).ToList();

                // todos los ejercicios
                List<EjercicioDTO> todosEjercicios = _rutinaServicio.ObtenerTodosEjercicios();

                // clientes asignados
                var idsClientesAsignados = _rutinaServicio.ObtenerAsignacionesPorRutina(id)
                                                         .Select(a => a.ClienteId)
                                                         .ToList();

                // todos los clientes
                List<ClienteDTO> todosClientes = _clienteServicio.ObtenerTodosDTO();

                // Armo DTO para la vista de edición
                var dto = new RutinaRegistroDTO
                {
                    Id = rutina.Id,
                    NombreRutina = rutina.NombreRutina,
                    Tipo = rutina.Tipo,

                    IdsEjerciciosSeleccionados = idsSeleccionados,
                    IdsClientesAsignados = idsClientesAsignados,

                    // separación entre mis ejercicios y los del sistema
                    MisEjerciciosDisponibles = todosEjercicios
                        .Where(e => e.ProfesionalId == profesionalId)
                        .ToList(),
                    EjerciciosDisponiblesSistema = todosEjercicios
                        .Where(e => e.ProfesionalId != profesionalId)
                        .ToList(),
                    EjerciciosSeleccionados = todosEjercicios
                        .Where(e => idsSeleccionados.Contains(e.Id))
                        .ToList(),

                    // clientes (todos / seleccionados)
                    ClientesDisponibles = todosClientes.ToList(),
                    ClientesSeleccionados = todosClientes
                        .Where(c => idsClientesAsignados.Contains(c.Id))
                        .ToList()
                };

                return View(dto);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionRutinas");
            }
        }

        [HttpPost]
        public IActionResult EditarRutina(RutinaRegistroDTO dto)
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

                // Seguridad + existencia
                Rutina rutina = _rutinaServicio.ObtenerRutinaPorId(dto.Id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para editar esta rutina.");

                // Datos base
                rutina.NombreRutina = dto.NombreRutina;
                rutina.Tipo = dto.Tipo;
                rutina.FechaModificacion = DateTime.Now;

                // Reemplazo ejercicios (ordeno según índice en la lista)
                rutina.Ejercicios.Clear();
                foreach (var ejercicioId in dto.IdsEjerciciosSeleccionados)
                {
                    rutina.Ejercicios.Add(new RutinaEjercicio
                    {
                        EjercicioId = ejercicioId,
                        Orden = dto.IdsEjerciciosSeleccionados.IndexOf(ejercicioId) + 1
                    });
                }

                // Reemplazo asignaciones de clientes
                _rutinaServicio.ReemplazarAsignaciones(rutina.Id, dto.IdsClientesAsignados);

                // Persistir en bd
                _rutinaServicio.ModificarRutina(rutina);

                TempData["Mensaje"] = "Rutina actualizada correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionRutinas");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionRutinas");
            }
        }

        [HttpPost]
        public IActionResult EliminarRutina(int id)
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

                // Seguridad y existencia
                Rutina rutina = _rutinaServicio.ObtenerRutinaPorId(id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para eliminar esta rutina.");
                if (rutina.Asignaciones.Any())
                    throw new Exception("No se puede eliminar una rutina que tienen clientes asignados. Debe desasignarlos primero.");

                // Eliminación
                bool ok = _rutinaServicio.EliminarRutina(id);
                if (!ok)
                    throw new Exception("No se pudo eliminar la rutina.");

                TempData["Mensaje"] = "Rutina eliminada correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionRutinas");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionRutinas");
            }
        }

        // =======================
        // Publicaciones (solicitudes del profesional)
        // =======================

        [HttpGet]
        public IActionResult SolicitarPublicacion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SolicitarPublicacion(CrearPublicacionDTO dto, List<IFormFile> ArchivosMedia)
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            try
            {
                // Armo la publicación a solicitar (queda pendiente o programada)
                Publicacion publicacion = new Publicacion
                {
                    Titulo = dto.Titulo,
                    Descripcion = dto.Descripcion,
                    FechaCreacion = DateTime.Now,
                    FechaProgramada = dto.FechaProgramada,
                    EsPrivada = dto.EsPrivada,
                    Estado = dto.FechaProgramada.HasValue ? Enum_EstadoPublicacion.Programada : Enum_EstadoPublicacion.Pendiente,
                    ProfesionalId = profesionalId,
                    ListaMedia = new List<Media>()
                };

                _publicacionServicio.CrearPublicacionImagenes(publicacion);

                // Agrego Adjuntos (si hay)
                if (ArchivosMedia != null && ArchivosMedia.Count > 0)
                {
                    foreach (var archivo in ArchivosMedia)
                    {
                        _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Publicacion, publicacion.Id);
                    }
                }

                TempData["Mensaje"] = "Se envio la solicitud para su revision";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("MisPublicaciones");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisPublicaciones");
            }
        }

        // Accedo a las publicaciones del profesional
        [HttpGet]
        public IActionResult MisPublicaciones()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            List<PublicacionDTO> publicaciones = _publicacionServicio.ObtenerPorProfesionalId(profesionalId);

            var modelo = new MisPublicacionesProfesional
            {
                Pendientes = publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Pendiente).ToList(),
                Aprobadas = publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Aprobada || p.Estado == Enum_EstadoPublicacion.Oculto).ToList(),
                Rechazadas = publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Rechazada).ToList()
            };

            return View(modelo);
        }

        // Detalle (Verificando que sea el autor)
        [HttpGet]
        public IActionResult DetallePublicacion(int id)
        {
            var publicacion = _publicacionServicio.ObtenerPorId(id);
            if (publicacion == null)
                return NotFound();

            int idSesion = GestionSesion.ObtenerUsuarioId(HttpContext);
            if (publicacion.RolAutor == "Profesional" && publicacion.AutorId != idSesion)
                return Forbid();

            return View(publicacion);
        }

        [HttpGet]
        public IActionResult EditarPublicacion(int id)
        {
            var publicacion = _publicacionServicio.ObtenerPorId(id);
            if (publicacion == null)
            {
                TempData["Mensaje"] = "La publicación no existe.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("ControlPublicaciones");
            }

            // Cargo enum para dropdown de estados
            ViewBag.Estados = Enum.GetValues(typeof(Enum_EstadoPublicacion))
                                  .Cast<Enum_EstadoPublicacion>()
                                  .Select(e => new SelectListItem
                                  {
                                      Value = ((int)e).ToString(),
                                      Text = e.ToString()
                                  }).ToList();

            return View(publicacion);
        }

        [HttpPost]
        public IActionResult EditarPublicacion(int Id, string Titulo, string Descripcion, List<IFormFile> archivos, bool Ocultar = false)
        {
            try
            {
                PublicacionDTO pub = _publicacionServicio.ObtenerPorId(Id);
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

                // Verifico: que sea el autor profesional
                if (pub == null || pub.AutorId != profesionalId && pub.RolAutor != "Profesional")
                    throw new Exception("No podés editar esta publicación.");

                // Actualizo datos
                pub.Titulo = Titulo;
                pub.Descripcion = Descripcion;
                pub.FechaProgramada = DateTime.Now;

                // Solo visible/oculto 
                pub.Estado = Ocultar ? Enum_EstadoPublicacion.Oculto
                                     : Enum_EstadoPublicacion.Aprobada;

                pub.MotivoRechazo = null;

                _publicacionServicio.ActualizarPublicacion(pub);

                // Nuevos adjuntos si llegan
                if (archivos != null && archivos.Any())
                {
                    foreach (var archivo in archivos)
                    {
                        _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Publicacion, pub.Id);
                    }
                }

                TempData["Mensaje"] = "Publicación editada correctamente. Queda pendiente de revisión.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("MisPublicaciones");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisPublicaciones");
            }
        }

        [HttpPost]
        public IActionResult EliminarMedia(int mediaId, int EjercicioId)
        {
            // Borrado simple de media asociada a un ejercicio
            _mediaServicio.EliminarMedia(mediaId);
            return RedirectToAction("EditarEjercicio", new { id = EjercicioId });
        }

        // =======================
        // Calendario del profesional (citas + agendas)
        // =======================

        [HttpGet]
        public IActionResult Calendario(int profesionalId)
        {
            // Ignoro el parámetro y tomo el de sesión para evitar fisuras
            int proId = GestionSesion.ObtenerUsuarioId(HttpContext);
            Profesional pro = _profesionalServicio.ObtenerProfesional(proId);

            // Preparo datos para el calendario (citas e intervalos de agenda)
            CargaCalendarioModelo modelo = new CargaCalendarioModelo
            {
                citas = _citaServicio.SolicitarHistorialProfesional(proId),
                agendas = pro.Agendas
            };

            return View(modelo);
        }

        // Devuelve JSON de eventos para pintar en el calendario
        public IActionResult ObtenerCitas(int profesionalId)
        {
            // Siempre uso el id de la sesión
            profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            List<Cita> citas = _citaServicio.SolicitarHistorialProfesional(profesionalId);

            var eventos = new List<object>();

            foreach (var c in citas)
            {
                // Colores del calendario según estado de la cita
                string color = "#808080"; // gris por defecto
                switch (c.Estado)
                {
                    case EstadoCita.EnEspera:
                        color = "#dc3545"; // rojo
                        break;
                    case EstadoCita.Aceptada:
                        color = "#007bff"; // azul
                        break;
                    case EstadoCita.Finalizada:
                        color = "#28a745"; // verde
                        break;
                    case EstadoCita.Cancelada:
                        color = "#6c757d"; // gris oscuro
                        break;
                    case EstadoCita.NoAsistio:
                        color = "#ffc107"; // amarillo
                        break;
                }

                // Fragmento mínimo que consume el frontend del calendario
                var citaFragmento = new
                {
                    id = c.Id,
                    title = $"Cita con {c.Cliente.NombreCompleto} | {c.TipoAtencion.Nombre}",
                    start = c.FechaAsistencia?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = c.FechaAsistencia?.AddMinutes(c.TipoAtencion.DuracionMin).ToString("yyyy-MM-ddTHH:mm:ss"),
                    color = color,
                    estado = c.Estado.ToString()
                };

                eventos.Add(citaFragmento);
            }

            return Json(eventos);
        }

        // =======================
        // Funciones extras
        // =======================

        // Chequea si una fecha cae dentro de una franja activa de agenda
        private bool EstaDentroDeFranja(List<AgendaProfesional> agendas, DateTime fecha)
        {
            Enum_DiaSemana diaSemana = ConvertirADiaSemana(fecha.DayOfWeek);
            var hora = fecha.TimeOfDay;

            return agendas
                .Where(a => a.Activo && a.Dia == diaSemana)
                .Any(a => hora >= a.HoraInicio && hora <= a.HoraFin);
        }

        // Traduce DayOfWeek de .NET al enum
        private Enum_DiaSemana ConvertirADiaSemana(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: return Enum_DiaSemana.Domingo;
                case DayOfWeek.Monday: return Enum_DiaSemana.Lunes;
                case DayOfWeek.Tuesday: return Enum_DiaSemana.Martes;
                case DayOfWeek.Wednesday: return Enum_DiaSemana.Miercoles;
                case DayOfWeek.Thursday: return Enum_DiaSemana.Jueves;
                case DayOfWeek.Friday: return Enum_DiaSemana.Viernes;
                case DayOfWeek.Saturday: return Enum_DiaSemana.Sabado;
                default: throw new Exception("revisar valores ingresados!");
            }
        }
    }
}
