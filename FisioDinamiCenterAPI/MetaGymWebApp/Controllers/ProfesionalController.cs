using Humanizer;
using LogicaApp.DTOS;
using LogicaApp.Servicios;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Repositorios;
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
            var model = new GestionCitasModelo
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
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult EditarCita(int id)
        {
            int ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var cita = citaServicio.ObtenerPorId(id);

            if (cita.Estado != EstadoCita.Aceptada)
            {
                TempData["Mensaje"] = "Solo se pueden editar citas aceptadas.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionCitas");
            }
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
                    cita.FechaFinalizacion = DateTime.Now;
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

        //Alta de ejercicio
        [HttpGet]
        public IActionResult RegistrarEjercicio()
        {
            return View(new EjercicioDTO());
        }
        [HttpPost]
        public IActionResult RegistrarEjercicio(EjercicioDTO dto, List<IFormFile> archivos)
        {
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
            //instancio la nueva rutina
            Rutina rutina = new Rutina
            {
                NombreRutina = dto.NombreRutina,
                Tipo = dto.Tipo,
                ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext),
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Ejercicios = dto.IdsEjerciciosSeleccionados.Select((id, index) => new RutinaEjercicio
                {
                    EjercicioId = id,
                    Orden = index + 1
                }).ToList()
            };
            try
            {
                // Guardar la rutina primero
                rutinaServicio.GenerarNuevaRutina(rutina);
                // Asignar a cada cliente
                foreach (var clienteId in dto.IdsClientesAsignados)
                {
                    rutinaServicio.AsignarRutinaACliente(clienteId, rutina.Id);
                }
                TempData["Mensaje"] = "Se registro la rutina correctamente, se asigno a los usuarios seleccionados.";
                TempData["TipoMensaje"] = "Success";
                return RedirectToAction("GestionRutinas");
            }
            catch (Exception)
            {
                TempData["Mensaje"] = "No se logro registrar la rutina, favor de intentar nuevamente mas tarde.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionRutinas");
            }

        }
        [HttpGet]
        public IActionResult EditarRutina(int id)
        {
            try
            {
                //Obtengo la rutina a modificar
                Rutina rutina = rutinaServicio.ObtenerRutinaPorId(id);
                //valido info
                if (rutina == null) throw new Exception("No se encontro rutina o no existe.");
                if (rutina.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar esta rutina.");
                //Mapeo a dto
                RutinaRegistroDTO dto = new RutinaRegistroDTO
                {
                    Id = rutina.Id,
                    NombreRutina = rutina.NombreRutina,
                    Tipo = rutina.Tipo,
                    IdsEjerciciosSeleccionados = rutina.Ejercicios.Select(e => e.EjercicioId).ToList(),
                    IdsClientesAsignados = rutinaServicio.ObtenerAsignacionesPorRutina(id).Select(a => a.ClienteId).ToList(),
                    MisEjerciciosDisponibles = rutinaServicio.ObtenerTodosEjercicios(),
                    ClientesDisponibles = clienteServicio.ObtenerTodosDTO()
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
                //Obtengo la rutina a modificar
                Rutina rutina = rutinaServicio.ObtenerRutinaPorId(dto.Id);
                //Valido info
                if (rutina == null) throw new Exception("No se encontro rutina o no existe.");
                //Verifico que quien la edita sea el dueño
                if (rutina.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar esta rutina.");

                //mapeo cambios
                rutina.NombreRutina = dto.NombreRutina;
                rutina.Tipo = dto.Tipo;
                rutina.FechaModificacion = DateTime.Now;
                rutina.Ejercicios = dto.IdsEjerciciosSeleccionados.Select((id, index) => new RutinaEjercicio
                {
                    EjercicioId = id,
                    Orden = index + 1
                }).ToList();
                //mando al repo
                rutinaServicio.ModificarRutina(rutina);

                // Reemplazar asignaciones
                rutinaServicio.ReemplazarAsignaciones(rutina.Id, dto.IdsClientesAsignados);

                TempData["Mensaje"] = "Se modifico la rutina correctamente.";
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
        public IActionResult MisPublicaciones()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            List<PublicacionDTO> publicaciones = publicacionServicio.ObtenerPorProfesionalId(profesionalId);

            return View(publicaciones);
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
    }
}


