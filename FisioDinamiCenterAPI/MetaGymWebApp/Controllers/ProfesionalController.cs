using LogicaApp.DTOS;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MetaGymWebApp.Controllers
{
    [AutorizacionRol("Profesional")]
    public class ProfesionalController : Controller
    {
        //Instancia
        private readonly IUsuarioServicio usuarioServicio;
        private readonly IProfesionalServicio profesionalServicio;
        private readonly ICitaServicio citaServicio;
        private readonly IExtraServicio extraServicio;
        private readonly IRutinaServicio rutinaServicio;
        private readonly IMediaServicio mediaServicio;
        private readonly IClienteServicio clienteServicio;
        private readonly IPublicacionServicio publicacionServicio;
        private readonly INotificacionServicio notificacionServicio;
        public ProfesionalController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IProfesionalServicio proservicio, IRutinaServicio rutina, IMediaServicio mediaServicio, IClienteServicio clienteServicio, IPublicacionServicio publicacionServicio,INotificacionServicio notificacion)
        {
            this.usuarioServicio = usuarioServicio;
            this.citaServicio = citaServicio;
            this.extraServicio = extraServicio;
            this.profesionalServicio = proservicio;
            this.rutinaServicio = rutina;
            this.mediaServicio = mediaServicio;
            this.clienteServicio = clienteServicio;
            this.publicacionServicio = publicacionServicio;
            this.notificacionServicio = notificacion;
        }
        [HttpGet]
        public IActionResult VerSolicitudCitas()
        {
            //Obtengo lista
            List<Cita> salida = citaServicio.BuscarPorEstado(EstadoCita.EnEspera);
            //Devuelvo
            return View(salida);

        }
        //Historiales clinicos
        [HttpGet]
        public IActionResult BuscarClientesCita()
        {
            var listaClientes = clienteServicio.ObtenerTodosDTO()
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
            Cita Cita = citaServicio.ObtenerPorId(id);
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
                Cita cita = citaServicio.ObtenerPorId(citaDTO.CitaId);

                if (cita.Estado != EstadoCita.EnEspera)
                    throw new Exception("La cita ya fue gestionada.");

                // Asignar profesional y cambiar estado
                cita.ProfesionalId = profesionalId;
                cita.Estado = EstadoCita.Aceptada;

                // Validar que esté dentro del horario profesional (esto lo implementás después si querés)
                cita.Validar();

                // Guardar cambios
                citaServicio.ActualizarEntidad(cita);

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
                List<int> tiene = profesionalServicio.ObtenerTiposAtencionProfesional(profesionalId);
                if (!tiene.Any())
                {
                    GestionCitasModelo salida = new GestionCitasModelo { TieneEspecialidades = false };
                    return View(salida);

                }
                else { 
                    // Obtener las citas correspondientes
                    var citasEnEspera = citaServicio
                    .BuscarSolicitudesSegunTiposAtencion(profesionalServicio.ObtenerTiposAtencionProfesional(profesionalId));

                var citasProximas = citaServicio
                    .SolicitarProximasProfesional(profesionalId);

                var citasFinalizadas = citaServicio
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
            var cita = citaServicio.ObtenerPorId(id);
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

            ViewBag.TiposAtencion = new SelectList(extraServicio.ObtenerTiposAtencionPorProfesional(ProfesionalId), "Id", "Nombre", dto.TipoAtencionId);
            return View(dto);
        }
        [HttpGet]
        public IActionResult EditarCitaParcial(int citaId)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var cita = citaServicio.ObtenerPorId(citaId);

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
                ProfesionalId = cita.ProfesionalId,
                EspecialidadId = cita.EspecialidadId,
                Descripcion = cita.Descripcion,
                TipoAtencionId = cita.TipoAtencionId
            };

            ViewBag.TiposAtencion = new SelectList(extraServicio.ObtenerTiposAtencionPorProfesional(ProfesionalId), "Id", "Nombre", dto.TipoAtencionId);
            return PartialView("Parciales/_EditarCitaParcial", dto);
        }
        [HttpPost]
        public IActionResult EditarCita(CitaDTO citaDTO)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            try
            {

                Cita cita = citaServicio.ObtenerPorId(citaDTO.CitaId);

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

                citaServicio.ActualizarEntidad(cita);

                TempData["Mensaje"] = "Cita actualizada correctamente.";
                return RedirectToAction("GestionCitas");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";

                ViewBag.TiposAtencion = new SelectList(extraServicio.ObtenerTiposAtencionPorProfesional(ProfesionalId), "Id", "Nombre", citaDTO.TipoAtencionId);
                return View(citaDTO);
            }
        }
        [HttpPost]
        public IActionResult EditarCitaCalendario(CitaDTO citaDTO)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            try
            {
                Cita cita = citaServicio.ObtenerPorId(citaDTO.CitaId);

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

                citaServicio.ActualizarEntidad(cita);

                return Json(new { success = true, message = "Cita actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult ReprogramarCita(int citaId, DateTime nuevaFecha)
        {
            Cita cita = citaServicio.ObtenerPorId(citaId);
            if (cita == null) return NotFound();

            cita.FechaAsistencia = nuevaFecha;
            citaServicio.ActualizarEntidad(cita);

            return Ok();
        }
        [HttpGet]
        public IActionResult HistorialClinicoCliente(int id)
        {
            List<CitaDTO> historial = citaServicio.SolicitarHistorialCliente(id)
                .Where(c => c.Estado == EstadoCita.Finalizada)
                .ToList();

            return View(historial);
        }

        [HttpGet] 
        public IActionResult HistorialSesionesCliente(int id)
        {
            List<SesionEntrenadaDTO> sesiones = rutinaServicio.ObtenerHistorialClienteDTO(id);
            return View(sesiones);
        }
        [HttpGet]
        public IActionResult GestionRutinas()
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                //Obtengo la lista de rutinas desde el repo
                List<Rutina> rutinas = rutinaServicio.ObtenerRutinasProfesional(profesionalId);
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
            var cita = citaServicio.ObtenerPorId(id);
            return View("VerCita", new CitaDTO
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
        public IActionResult VerCitaParcial(int citaId)
        {
            var cita = citaServicio.ObtenerPorId(citaId);
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
                Clientes = clienteServicio.ObtenerTodosDTO(),
                Especialidades = profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId),
                TiposAtencion = extraServicio.ObtenerTiposAtencionPorProfesionalDTO(profesionalId),
                Establecimientos = extraServicio.ObtenerEstablecimientosDTO(),
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
                Clientes = clienteServicio.ObtenerTodosDTO(),
                Especialidades = profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId),
                TiposAtencion = extraServicio.ObtenerTiposAtencionPorProfesionalDTO(profesionalId),
                Establecimientos = extraServicio.ObtenerEstablecimientosDTO(),
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
                Profesional profesional = profesionalServicio.ObtenerProfesional(GestionSesion.ObtenerUsuarioId(HttpContext));
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

                citaServicio.RegistrarCitaPorProfesional(cita);
                bool dentroFranja = EstaDentroDeFranja(profesional.Agendas, cita.FechaAsistencia);
                if (!dentroFranja)
                    throw new Exception("La fecha seleccionada no está dentro de la franja horaria asignada.");

                return Json(new { success = true, mensaje = "Cita registrada correctamente." });
            }
            catch (Exception ex)
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                modelo.Clientes = clienteServicio.ObtenerTodosDTO();
                modelo.Especialidades = profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId);
                modelo.TiposAtencion = extraServicio.ObtenerTiposAtencionPorProfesionalDTO(profesionalId);
                modelo.Establecimientos = extraServicio.ObtenerEstablecimientosDTO();

                return PartialView("Parciales/_CrearCitaParcial", modelo);
            }
        }

        [HttpPost]
        public IActionResult CrearCita(RegistroCitaModelo modelo)
        {
            try
            {
                CitaDTO cita = modelo.Cita;
                cita.ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
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
                citaServicio.RegistrarCitaPorProfesional(cita);


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
                modelo.Clientes = clienteServicio.ObtenerTodosDTO();
                modelo.Especialidades = profesionalServicio.ObtenerEspecialidadesProfesionalDTO(profesionalId);
                modelo.TiposAtencion = extraServicio.ObtenerTiposAtencionPorProfesionalDTO(profesionalId);
                modelo.Establecimientos = extraServicio.ObtenerEstablecimientosDTO();

                return View(modelo);
            }
        }


        //Ejercicios
        [HttpGet]
        public IActionResult GestionEjercicios()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            //Obtengo la lista de ejercicios
            List<EjercicioDTO> todos = rutinaServicio.ObtenerTodosEjercicios();

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
            var listaClientes = clienteServicio.ObtenerTodosDTO()
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
                EjercicioDTO ejercicio = rutinaServicio.ObtenerEjercicioDTOId(id);
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
                EjercicioDTO ejercicio = rutinaServicio.ObtenerEjercicioDTOId(id);
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
                rutinaServicio.GenerarNuevoEjercicio(ejercicio); 
                //Si salio bien, ahora cargo los archivos.
                if (archivos != null && archivos.Count > 0)
                {
                    foreach (var archivo in archivos)
                    {
                        var media = mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Ejercicio, ejercicio.Id);
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
                EjercicioDTO dto = rutinaServicio.ObtenerEjercicioDTOId(id);
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
                Ejercicio ejercicio = rutinaServicio.ObtenerEjercicioId(dto.Id);
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
                        var media = mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Ejercicio, ejercicio.Id);
                        ejercicio.Medias.Add(media);
                    }
                }
                rutinaServicio.ModificarEjercicio(ejercicio);
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
            List<EjercicioDTO> todos = rutinaServicio.ObtenerTodosEjercicios();
            //genero modelo para la vista
            RutinaRegistroDTO modelo = new RutinaRegistroDTO
            {
                MisEjerciciosDisponibles = todos.Where(e => e.ProfesionalId == profesionalId).ToList(),
                EjerciciosDisponiblesSistema = todos.Where(e => e.ProfesionalId != profesionalId).ToList(),
                ClientesDisponibles = clienteServicio.ObtenerTodosDTO()
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
                rutinaServicio.GenerarNuevaRutina(rutina);

                // Agregar ejercicios usando el mismo patrón de merge
                rutinaServicio.ActualizarEjerciciosRutina(rutina, dto.IdsEjerciciosSeleccionados);

                // Asignar a cada cliente
                foreach (var clienteId in dto.IdsClientesAsignados)
                {
                    rutinaServicio.AsignarRutinaACliente(clienteId, rutina.Id);
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
                Rutina rutina = rutinaServicio.ObtenerRutinaPorId(id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para editar esta rutina.");

                // Ejercicios de la rutina
                var idsSeleccionados = rutina.Ejercicios.Select(e => e.EjercicioId).ToList();

                // Todos los ejercicios
                List<EjercicioDTO> todosEjercicios = rutinaServicio.ObtenerTodosEjercicios();

                // Clientes asignados a la rutina
                var idsClientesAsignados = rutinaServicio.ObtenerAsignacionesPorRutina(id)
                                                         .Select(a => a.ClienteId)
                                                         .ToList();

                // Todos los clientes
                List<ClienteDTO> todosClientes = clienteServicio.ObtenerTodosDTO();

                // Armar DTO para la vista
                var dto = new RutinaRegistroDTO
                {
                    Id = rutina.Id,
                    NombreRutina = rutina.NombreRutina,
                    Tipo = rutina.Tipo,

                    IdsEjerciciosSeleccionados = idsSeleccionados,
                    IdsClientesAsignados = idsClientesAsignados,

                    // Ejercicios disponibles (se excluyen los ya seleccionados)
                    MisEjerciciosDisponibles = todosEjercicios
                        .Where(e => e.ProfesionalId == profesionalId && !idsSeleccionados.Contains(e.Id))
                        .ToList(),

                    EjerciciosDisponiblesSistema = todosEjercicios
                        .Where(e => e.ProfesionalId != profesionalId && !idsSeleccionados.Contains(e.Id))
                        .ToList(),

                    // Ejercicios seleccionados (solo los de la rutina)
                    EjerciciosSeleccionados = todosEjercicios
                        .Where(e => idsSeleccionados.Contains(e.Id))
                        .ToList(),

                    // Clientes disponibles (sin los ya asignados)
                    ClientesDisponibles = todosClientes
                        .Where(c => !idsClientesAsignados.Contains(c.Id))
                        .ToList(),

                    // Clientes seleccionados
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
                Rutina rutina = rutinaServicio.ObtenerRutinaPorId(dto.Id);
                if (rutina == null)
                    throw new Exception("No se encontró rutina o no existe.");
                if (rutina.ProfesionalId != profesionalId)
                    throw new Exception("No tiene permisos para editar esta rutina.");

                // Actualizar datos principales
                rutina.NombreRutina = dto.NombreRutina;
                rutina.Tipo = dto.Tipo;
                rutina.FechaModificacion = DateTime.Now;

                // === Actualizar ejercicios ===
                rutina.Ejercicios.Clear();
                foreach (var ejercicioId in dto.IdsEjerciciosSeleccionados)
                {
                    rutina.Ejercicios.Add(new RutinaEjercicio
                    {
                        EjercicioId = ejercicioId,
                        Orden = dto.IdsEjerciciosSeleccionados.IndexOf(ejercicioId) + 1
                    });
                }

                // === Actualizar clientes asignados ===
                rutinaServicio.ReemplazarAsignaciones(rutina.Id, dto.IdsClientesAsignados);

                // Guardar cambios
                rutinaServicio.ModificarRutina(rutina);

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
                publicacionServicio.CrearPublicacionImagenes(publicacion);
                //Si hay imagenes las registro
                if (ArchivosMedia != null && ArchivosMedia.Count > 0)
                {
                    foreach (var archivo in ArchivosMedia)
                    {
                        mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Publicacion, publicacion.Id);
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
        [HttpGet]
        public IActionResult MisPublicaciones()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            List<PublicacionDTO> publicaciones = publicacionServicio.ObtenerPorProfesionalId(profesionalId);

            var modelo = new MisPublicacionesProfesional
            {
                Pendientes = publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Pendiente).ToList(),
                Aprobadas = publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Aprobada).ToList(),
                Rechazadas = publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Rechazada).ToList()
            };

            return View(modelo);
        }

        //Detales
        [HttpGet]
        public IActionResult DetallePublicacion(int id)
        {
            var publicacion = publicacionServicio.ObtenerPorId(id);
            if (publicacion == null)
                return NotFound();

            // Validación: debe pertenecer al profesional logueado
            int idSesion = GestionSesion.ObtenerUsuarioId(HttpContext);
            if (publicacion.RolAutor == "Profesional" && publicacion.AutorId != idSesion)
                return Forbid(); // o RedirectToAction("MisPublicaciones");

            return View(publicacion);
        }
        [HttpGet]
        public IActionResult EditarPublicacion(int id)
        {
            //Obtengo publicacion desde repo
            PublicacionDTO publicacion = publicacionServicio.ObtenerPorId(id);
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            //Verifico que no sea null. Y que corresponda al autor que la solicita 
            if (publicacion == null || publicacion.AutorId != profesionalId || publicacion.RolAutor != "Profesional")
            {
                TempData["Mensaje"] = "No tenés permiso para editar esta publicación.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisPublicaciones");
            }
            return View(publicacion);
        }
        [HttpPost]
        public IActionResult EditarPublicacion(int Id, string Titulo, string Descripcion, List<IFormFile> archivos)
        {
            try
            {
                //Obtengo publicacion por repo
                PublicacionDTO pub = publicacionServicio.ObtenerPorId(Id);
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

                //Si no es el dueño lo devuevlo al menu
                if (pub == null || pub.AutorId != profesionalId && pub.RolAutor != "Profesional")
                    throw new Exception("No podés editar esta publicación.");

                //Mapeo cambiois
                pub.Titulo = Titulo;
                pub.Descripcion = Descripcion;
                pub.FechaProgramada = DateTime.Now;
                //Queda pendiente de ser aprobada
                pub.Estado = Enum_EstadoPublicacion.Pendiente;
                pub.MotivoRechazo = null;
                //Mando al repo
                publicacionServicio.ActualizarPublicacion(pub);
                //Cargo las fotoso nuevas si corresponde.
                if (archivos != null && archivos.Any())
                {
                    foreach (var archivo in archivos)
                    {
                        mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Publicacion, pub.Id);
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
            mediaServicio.EliminarMedia(mediaId);
            return RedirectToAction("EditarEjercicio", new { id = EjercicioId });
        }

        //Calendarios prueba
        public IActionResult Calendario(int profesionalId)
        {
            int proId = GestionSesion.ObtenerUsuarioId(HttpContext);
            List<Cita> citas = citaServicio.SolicitarProximasProfesional(proId);
            return View(citas);
        }

        // Endpoint para alimentar FullCalendar con JSON
        public IActionResult ObtenerCitas(int profesionalId)
        {
            profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            List<Cita> citas = citaServicio.SolicitarHistorialProfesional(profesionalId);

            var eventos = new List<object>();

            foreach (var c in citas)
            {
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
            foreach (var a in agendas)
            {
                if (a.Dia == ConvertirADiaSemana(fecha.DayOfWeek) &&
                    fecha.TimeOfDay >= a.HoraInicio &&
                    fecha.TimeOfDay < a.HoraFin)
                {
                    return true;
                }
            }
            return false;
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
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }


}


