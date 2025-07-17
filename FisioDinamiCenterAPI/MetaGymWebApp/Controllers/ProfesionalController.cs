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
        public ProfesionalController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IProfesionalServicio proservicio, IRutinaServicio rutina, IMediaServicio mediaServicio, IClienteServicio clienteServicio, IPublicacionServicio publicacionServicio)
        {
            this.usuarioServicio = usuarioServicio;
            this.citaServicio = citaServicio;
            this.extraServicio = extraServicio;
            this.profesionalServicio = proservicio;
            this.rutinaServicio = rutina;
            this.mediaServicio = mediaServicio;
            this.clienteServicio = clienteServicio;
            this.publicacionServicio = publicacionServicio;
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
            //Valido

            //Mando a repo

            //Redireccion
            return View();
        }

        public IActionResult GestionCitas(EstadoCita estado = EstadoCita.EnEspera)
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

            List<Cita> citas;

            if (estado == EstadoCita.EnEspera)
            {
                citas = citaServicio.BuscarSolicitudesSegunEspecialidades(profesionalServicio.ObtenerEspecialidadesProfesional(profesionalId));
            }
            else if (estado == EstadoCita.Aceptada)
            {
                citas = citaServicio.SolicitarProximasProfesional(profesionalId);
            }
            else if (estado == EstadoCita.Finalizada)
            {
                citas = citaServicio.SolicitarHistorialProfesional(profesionalId);
            }
            else
            {
                citas = new List<Cita>();
            }
            //Gestion de rutinas
            // Mapear a DTOs
            var dtoList = citas.Select(c => new CitaDTO
            {
                CitaId = c.Id,
                Cliente = c.Cliente,
                Especialidad = c.Especialidad,
                Establecimiento = c.Establecimiento,
                Descripcion = c.Descripcion,
                FechaAsistencia = c.FechaAsistencia.Value,
                FechaCreacion = c.FechaCreacion,
                ProfesionalId = c.ProfesionalId,
                Conclusion = c.Conclusion
            }).ToList();

            var model = new GestionCitasModelo
            {
                Citas = dtoList,
                EstadoSeleccionado = estado
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult GestionRutinas()
        {
            try
            {
                int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                var rutinas = rutinaServicio.ObtenerRutinasProfesional(profesionalId);
                if (rutinas == null) throw new Exception("No se obtuvieron las rutinas, intente iniciando sesion nuevamente.");
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
        //Ejercicios
        [HttpGet]
        public IActionResult GestionEjercicios()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);

            var todos = rutinaServicio.ObtenerTodosEjercicios();

            var modelo = new GestionEjerciciosModelo
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
                EjercicioDTO ejercicio = rutinaServicio.ObtenerEjercicioDTOId(id);
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
            var ejercicio = new Ejercicio
            {
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                GrupoMuscular = dto.GrupoMuscular,
                Instrucciones = dto.Instrucciones,
                ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext),
                Medias = new List<Media>()
            };
            try
            {
                rutinaServicio.GenerarNuevoEjercicio(ejercicio); // primero guardamos para obtener el ID
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
                var dto = rutinaServicio.ObtenerEjercicioDTOId(id);
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
                Ejercicio ejercicio = rutinaServicio.ObtenerEjercicioId(dto.Id);
                if (ejercicio == null) throw new Exception("No se encontro ejercicio o no existe.");
                if (dto.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar este ejercicio.");
                // Actualizar datos principales
                ejercicio.Nombre = dto.Nombre;
                ejercicio.Tipo = dto.Tipo;
                ejercicio.GrupoMuscular = dto.GrupoMuscular;
                ejercicio.Instrucciones = dto.Instrucciones;

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
            //ViewBag.Ejercicios = rutinaServicio.ObtenerTodosEjercicios(); // EjercicioDTO
            var modelo = new RutinaRegistroDTO
            {
                EjerciciosDisponibles = rutinaServicio.ObtenerTodosEjercicios(),
                ClientesDisponibles = clienteServicio.ObtenerTodosDTO()
            };

            return View(modelo);
        }
            [HttpPost]
            public IActionResult RegistrarRutina(RutinaRegistroDTO dto)
        {
            var rutina = new Rutina
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
                var rutina = rutinaServicio.ObtenerRutinaPorId(id);
                if (rutina == null) throw new Exception("No se encontro rutina o no existe.");
                if (rutina.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar esta rutina.");
                var dto = new RutinaRegistroDTO
                {
                    Id = rutina.Id,
                    NombreRutina = rutina.NombreRutina,
                    Tipo = rutina.Tipo,
                    IdsEjerciciosSeleccionados = rutina.Ejercicios.Select(e => e.EjercicioId).ToList(),
                    IdsClientesAsignados = rutinaServicio.ObtenerAsignacionesPorRutina(id).Select(a => a.ClienteId).ToList(),
                    EjerciciosDisponibles = rutinaServicio.ObtenerTodosEjercicios(),
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
                var rutina = rutinaServicio.ObtenerRutinaPorId(dto.Id);
                if (rutina == null) throw new Exception("No se encontro rutina o no existe.");
                if (rutina.ProfesionalId != GestionSesion.ObtenerUsuarioId(this.HttpContext)) throw new Exception("No tiene permisos para editar esta rutina.");
                rutina.NombreRutina = dto.NombreRutina;
                rutina.Tipo = dto.Tipo;
                rutina.FechaModificacion = DateTime.Now;
                rutina.Ejercicios = dto.IdsEjerciciosSeleccionados.Select((id, index) => new RutinaEjercicio
                {
                    EjercicioId = id,
                    Orden = index + 1
                }).ToList();

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
           
                //Lo creo
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
    }
    }


