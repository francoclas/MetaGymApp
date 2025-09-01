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
    [AutorizacionRol("Profesional")]
    public class ProfesionalController : Controller
    {
        //Instancia
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IProfesionalServicio _profesionalServicio;
        private readonly ICitaServicio _citaServicio;
        private readonly IExtraServicio _extraServicio;
        private readonly IRutinaServicio _rutinaServicio;
        private readonly IMediaServicio _mediaServicio;
        private readonly IClienteServicio _clienteServicio;
        private readonly IPublicacionServicio _publicacionServicio;
        private readonly INotificacionServicio _notificacionServicio;
        public ProfesionalController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IProfesionalServicio proservicio, IRutinaServicio rutina, IMediaServicio mediaServicio, IClienteServicio clienteServicio, IPublicacionServicio publicacionServicio,INotificacionServicio notificacion)
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
        [HttpGet]
        public IActionResult VerSolicitudCitas()
        {
            //Obtengo lista
            List<Cita> salida = _citaServicio.BuscarPorEstado(EstadoCita.EnEspera);
            //Devuelvo
            return View(salida);

        }
        //Historiales clinicos
        [HttpGet]
        public IActionResult BuscarClientesCita()
        {
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
        //Gestionar cita
        [HttpGet]
        public IActionResult RevisarCita(int id)
        {
            //Obtengo cita
            Cita Cita = _citaServicio.ObtenerPorId(id);
            //Cargo 
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

                // Obtener la cita original
                Cita cita = _citaServicio.ObtenerPorId(citaDTO.CitaId);

                if (cita.Estado != EstadoCita.EnEspera)
                    throw new Exception("La cita ya fue gestionada.");

                // Asignar profesional y cambiar estado
                cita.ProfesionalId = profesionalId;
                cita.Estado = EstadoCita.Aceptada;

                // Validar que esté dentro del horario profesional (esto lo implementás después si querés)
                cita.Validar();

                // Guardar cambios
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

        public IActionResult GestionCitas()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

            try
            {
                //Valido si usuariorio tiene especialidades
                List<int> tiene = _profesionalServicio.ObtenerTiposAtencionProfesional(profesionalId);
                if (!tiene.Any())
                {
                    GestionCitasModelo salida = new GestionCitasModelo { TieneEspecialidades = false };
                    return View(salida);

                }
                else { 
                    // Obtener las citas correspondientes
                    var citasEnEspera = _citaServicio
                    .BuscarSolicitudesSegunTiposAtencion(_profesionalServicio.ObtenerTiposAtencionProfesional(profesionalId));

                var citasProximas = _citaServicio
                    .SolicitarProximasProfesional(profesionalId);

                var citasFinalizadas = _citaServicio
                    .SolicitarHistorialProfesional(profesionalId)
                    .Where(c => c.Estado == EstadoCita.Finalizada)
                    .ToList();

                // Mapeo a DTOs
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
            catch (Exception)
            {

                throw;
            }
            
        }
        [HttpGet]
        public IActionResult EditarCita(int id)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var cita = _citaServicio.ObtenerPorId(id);
            if (cita.ProfesionalId != ProfesionalId)
            {
                TempData["Mensaje"] = "No tenes asignada la rutina a editar";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionCitas");

            }
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
                if (cita.ProfesionalId != ProfesionalId)
                {
                    TempData["Mensaje"] = "No tenes asignada la rutina a editar";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("GestionCitas");

                }
                // Validaciones según nuevo estado
                if (citaDTO.Estado == EstadoCita.Finalizada ||
                    citaDTO.Estado == EstadoCita.Cancelada ||
                    citaDTO.Estado == EstadoCita.NoAsistio ||
                    citaDTO.Estado == EstadoCita.Rechazada)
                {
                    if (string.IsNullOrWhiteSpace(citaDTO.Conclusion))
                        throw new Exception("Debe ingresar una conclusión para finalizar o cancelar la cita.");

                    cita.Conclusion = citaDTO.Conclusion;
                    cita.Estado = citaDTO.Estado;
                    if (citaDTO.Estado == EstadoCita.Finalizada)
                    {
                        cita.FechaFinalizacion = DateTime.Now;

                    }
                }
                else
                {
                    cita.Estado = citaDTO.Estado; // Aceptada u otro cambio
                }

                // Fecha o descripción
                cita.FechaAsistencia = citaDTO.FechaAsistencia;
                cita.Descripcion = citaDTO.Descripcion;

                // Tipo atención si vino
                if (citaDTO.TipoAtencionId.HasValue)
                    cita.TipoAtencionId = citaDTO.TipoAtencionId.Value;

                cita.Validar(); // si usás validaciones internas

                _citaServicio.ActualizarEntidad(cita);

                TempData["Mensaje"] = "Cita actualizada correctamente.";
                return RedirectToAction("GestionCitas");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";

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
            nuevaFecha = DateTime.SpecifyKind(nuevaFecha, DateTimeKind.Local);
            try
            {
                Cita cita = _citaServicio.ObtenerPorId(citaId);
                Profesional profesional = _profesionalServicio.ObtenerProfesional(GestionSesion.ObtenerUsuarioId(HttpContext));
                if (cita == null) throw new Exception("No se encontro cita.");
                if (cita.Estado != EstadoCita.Aceptada) throw new Exception("Solo se puede reprogramar una cita Aceptada.");
                bool dentroFranja = EstaDentroDeFranja(profesional.Agendas, nuevaFecha);
                if (!dentroFranja) throw new Exception("No se puede reprogramar la cita a esa fecha, no hay jornada laboral en esa hora.");
                    cita.FechaAsistencia = nuevaFecha;
                _citaServicio.ActualizarEntidad(cita);
                _notificacionServicio.NotificacionPersonalizada(cita.ClienteId,"Cliente", 
                    new Notificacion { 
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
        [HttpGet]
        public IActionResult HistorialClinicoCliente(int id)
        {
            List<CitaDTO> historial = _citaServicio.SolicitarHistorialCliente(id)
                .Where(c => c.Estado == EstadoCita.Finalizada)
                .ToList();

            return View(historial);
        }

        [HttpGet] 
        public IActionResult HistorialSesionesCliente(int id)
        {
            List<SesionEntrenadaDTO> sesiones = _rutinaServicio.ObtenerHistorialClienteDTO(id);
            return View(sesiones);
        }
        [HttpGet]
        public IActionResult GestionRutinas()
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                //Obtengo la lista de rutinas desde el repo
                List<Rutina> rutinas = _rutinaServicio.ObtenerRutinasProfesional(profesionalId);
                //Valido que exista
                if (rutinas == null) throw new Exception("No se obtuvieron las rutinas, intente iniciando sesion nuevamente.");
                //Devuelvo mapeando la lista de las rutinas
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
                return RedirectToAction("Inicio","Publicacion");
            }
            
        }
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
        [HttpGet]
        public IActionResult CrearCita()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

            RegistroCitaModelo modelo = new RegistroCitaModelo
            {
                Clientes = _clienteServicio.ObtenerTodosDTO(),

                // 🔹 Este ya trae los tipos de atención dentro de cada especialidad
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
                //Valido la info ingresada
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
                //mando al repo
                bool dentroFranja = EstaDentroDeFranja(profesional.Agendas, cita.FechaAsistencia);
                if (!dentroFranja)
                    throw new Exception("La fecha seleccionada no está dentro de la franja horaria asignada.");
                _citaServicio.RegistrarCitaPorProfesional(cita);

                TempData["Mensaje"] = "Cita registrada correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionCitas");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";

                // Recargar listas
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                modelo.Clientes = _clienteServicio.ObtenerTodosDTO();
                modelo.Especialidades = _profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId);
                modelo.TiposAtencion = _extraServicio.ObtenerTiposAtencionPorProfesionalDTO(profesionalId);
                modelo.Establecimientos = _extraServicio.ObtenerEstablecimientosDTO();

                return View(modelo);
            }
        }


        //Ejercicios
        [HttpGet]
        public IActionResult GestionEjercicios()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            //Obtengo la lista de ejercicios
            List<EjercicioDTO> todos = _rutinaServicio.ObtenerTodosEjercicios();

            //Mapeo para el modelo
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
                //Obtengo el ejercicio a mostrar desde el repo
                EjercicioDTO ejercicio = _rutinaServicio.ObtenerEjercicioDTOId(id);
                //Verifico que existe
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
                //Obtengo el ejercicio a mostrar desde el repo
                EjercicioDTO ejercicio = _rutinaServicio.ObtenerEjercicioDTOId(id);
                //Verifico que existe
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
        //Alta de ejercicio
        [HttpGet]
        public IActionResult RegistrarEjercicio()
        {
            return View(new EjercicioDTO());
        }
        [HttpPost]
        public IActionResult RegistrarEjercicio(EjercicioDTO dto, List<IFormFile> archivos)
        {
            //valido datos
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
            //Instancio el ejercicio a registrar
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
                //lo mando al repo para tener id
                _rutinaServicio.GenerarNuevoEjercicio(ejercicio); 
                //Si salio bien, ahora cargo los archivos.
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

        //Editar ejercicio
        [HttpGet]
        public IActionResult EditarEjercicio(int id)
        {
            try
            {
                //obtengo del repo y mapeo al dto
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
                //Obtengo el ejercicio desde repo
                Ejercicio ejercicio = _rutinaServicio.ObtenerEjercicioId(dto.Id);
                if (ejercicio == null) throw new Exception("No se encontro ejercicio o no existe.");
                if (dto.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar este ejercicio.");
                //Mapeo los cambios
                ejercicio.Nombre = dto.Nombre;
                ejercicio.Tipo = dto.Tipo;
                ejercicio.GrupoMuscular = dto.GrupoMuscular;
                ejercicio.Instrucciones = dto.Instrucciones;
                ejercicio.Mediciones = dto.Mediciones;
                // Agregar nuevos archivos 
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
                // Buscar ejercicio
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

        [HttpGet]
        public IActionResult RegistrarRutina()
        {
            //Obtengo ejercicois para listar
            int profesionalId = GestionSesion.ObtenerUsuarioId(this.HttpContext);
            List<EjercicioDTO> todos = _rutinaServicio.ObtenerTodosEjercicios();
            //genero modelo para la vista
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
                // Crear la rutina vacía
                var rutina = new Rutina
                {
                    NombreRutina = dto.NombreRutina,
                    Tipo = dto.Tipo,
                    ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext),
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    Ejercicios = new List<RutinaEjercicio>() // empezamos vacío
                };

                // Guardar la rutina primero para obtener Id
                _rutinaServicio.GenerarNuevaRutina(rutina);

                // Agregar ejercicios usando el mismo patrón de merge
                _rutinaServicio.ActualizarEjerciciosRutina(rutina, dto.IdsEjerciciosSeleccionados);

                // Asignar a cada cliente
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

                // Obtener la rutina
                Rutina rutina = _rutinaServicio.ObtenerRutinaPorId(id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para editar esta rutina.");

                // ejercicios de la rutina
                var idsSeleccionados = rutina.Ejercicios.Select(e => e.EjercicioId).ToList();

                // todos los ejercicios
                List<EjercicioDTO> todosEjercicios = _rutinaServicio.ObtenerTodosEjercicios();

                // clientes asignados a la rutina
                var idsClientesAsignados = _rutinaServicio.ObtenerAsignacionesPorRutina(id)
                                                         .Select(a => a.ClienteId)
                                                         .ToList();

                // todos los clientes
                List<ClienteDTO> todosClientes = _clienteServicio.ObtenerTodosDTO();

                // Armar DTO para la vista
                var dto = new RutinaRegistroDTO
                {
                    Id = rutina.Id,
                    NombreRutina = rutina.NombreRutina,
                    Tipo = rutina.Tipo,

                    IdsEjerciciosSeleccionados = idsSeleccionados,
                    IdsClientesAsignados = idsClientesAsignados,

                    //filtro los ejercicios
                    MisEjerciciosDisponibles = todosEjercicios
                        .Where(e => e.ProfesionalId == profesionalId)
                        .ToList(),
                    EjerciciosDisponiblesSistema = todosEjercicios
                        .Where(e => e.ProfesionalId != profesionalId)
                        .ToList(),
                    EjerciciosSeleccionados = todosEjercicios
                        .Where(e => idsSeleccionados.Contains(e.Id))
                        .ToList(),
                    //filtro los clientes
                    ClientesDisponibles = todosClientes                   
                        .ToList(),
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

                // Buscar la rutina
                Rutina rutina = _rutinaServicio.ObtenerRutinaPorId(dto.Id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para editar esta rutina.");

                //Actualizar datos principales
                rutina.NombreRutina = dto.NombreRutina;
                rutina.Tipo = dto.Tipo;
                rutina.FechaModificacion = DateTime.Now;

                //Actualizar ejercicios ===
                rutina.Ejercicios.Clear();
                foreach (var ejercicioId in dto.IdsEjerciciosSeleccionados)
                {
                    rutina.Ejercicios.Add(new RutinaEjercicio
                    {
                        EjercicioId = ejercicioId,
                        Orden = dto.IdsEjerciciosSeleccionados.IndexOf(ejercicioId) + 1
                    });
                }

                //Actualizar clientes asignados
                _rutinaServicio.ReemplazarAsignaciones(rutina.Id, dto.IdsClientesAsignados);

                // Guardar cambios
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

                // Buscar la rutina
                Rutina rutina = _rutinaServicio.ObtenerRutinaPorId(id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para eliminar esta rutina.");
                if (rutina.Asignaciones.Any())
                    throw new Exception("No se puede eliminar una rutina que tienen clientes asignados. Debe desasignarlos primero.");
                // Eliminar
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

        //Publicaciones

        //Solicitar publicacion
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
                //valido datos
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
           
                //Envio al repo
                _publicacionServicio.CrearPublicacionImagenes(publicacion);
                //Si hay imagenes las registro
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
        //Mis publicaciones
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

        //Detales
        [HttpGet]
        public IActionResult DetallePublicacion(int id)
        {
            var publicacion = _publicacionServicio.ObtenerPorId(id);
            if (publicacion == null)
                return NotFound();

            // Valido que sea el creador
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

            // Cargo enum en ViewBag
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
                //Obtengo publicacion por repo
                PublicacionDTO pub = _publicacionServicio.ObtenerPorId(Id);
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

                //Si no es el dueño lo devuevlo al menu
                if (pub == null || pub.AutorId != profesionalId && pub.RolAutor != "Profesional")
                    throw new Exception("No podés editar esta publicación.");

                //Mapeo cambiois
                pub.Titulo = Titulo;
                pub.Descripcion = Descripcion;
                pub.FechaProgramada = DateTime.Now;
                // Solo visible u oculto
                pub.Estado = Ocultar ? Enum_EstadoPublicacion.Oculto
                                     : Enum_EstadoPublicacion.Aprobada;

                pub.MotivoRechazo = null;
                //Mando al repo
                _publicacionServicio.ActualizarPublicacion(pub);
                //Cargo las fotoso nuevas si corresponde.
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
            _mediaServicio.EliminarMedia(mediaId);
            return RedirectToAction("EditarEjercicio", new { id = EjercicioId });
        }

        //Calendarios prueba

        [HttpGet]
        public IActionResult Calendario(int profesionalId)
        {
            int proId = GestionSesion.ObtenerUsuarioId(HttpContext);
            Profesional pro = _profesionalServicio.ObtenerProfesional(proId);
            CargaCalendarioModelo modelo = new CargaCalendarioModelo
            {
                citas = _citaServicio.SolicitarHistorialProfesional(proId),
                agendas = pro.Agendas
            };

        return View(modelo);
        }

        public IActionResult ObtenerCitas(int profesionalId)
        {
            profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            List<Cita> citas = _citaServicio.SolicitarHistorialProfesional(profesionalId);

            var eventos = new List<object>();

            foreach (var c in citas)
            {
                //mapeo los colores para el calendairo
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


        private bool EstaDentroDeFranja(List<AgendaProfesional> agendas, DateTime fecha)
        {
            Enum_DiaSemana diaSemana = ConvertirADiaSemana(fecha.DayOfWeek);
            var hora = fecha.TimeOfDay;

            return agendas
                .Where(a => a.Activo && a.Dia == diaSemana)
                .Any(a => hora >= a.HoraInicio && hora <= a.HoraFin);
        }
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


