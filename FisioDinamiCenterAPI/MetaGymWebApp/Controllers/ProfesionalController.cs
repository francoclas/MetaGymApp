using LogicaApp.DTOS;
using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
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
        public ProfesionalController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IProfesionalServicio proservicio, IRutinaServicio rutina, IMediaServicio mediaServicio, IClienteServicio clienteServicio)
        {
            this.usuarioServicio = usuarioServicio;
            this.citaServicio = citaServicio;
            this.extraServicio = extraServicio;
            this.profesionalServicio = proservicio;
            this.rutinaServicio = rutina;
            this.mediaServicio = mediaServicio;
            this.clienteServicio = clienteServicio;
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
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var rutinas = rutinaServicio.ObtenerRutinasProfesional(profesionalId);

            return View(rutinas.Select(r => new RutinaDTO
            {
                Id = r.Id,
                NombreRutina = r.NombreRutina,
                Tipo = r.Tipo,
                UltimaModificacion = DateTime.Now
            }).ToList());
        }
        //Ejercicios
        [HttpGet]
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
        [HttpGet]
        public IActionResult DetalleEjercicio(int id)
        {
            EjercicioDTO ejercicio = rutinaServicio.ObtenerEjercicioDTOId(id);
            if (ejercicio == null) return NotFound();

            return View(ejercicio);
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

            if (archivos != null && archivos.Count > 0)
            {
                foreach (var archivo in archivos)
                {
                    if (archivo.Length > 0)
                    {
                        var extension = Path.GetExtension(archivo.FileName).ToLower();
                        var tipo = extension switch
                        {
                            ".jpg" or ".jpeg" or ".png" => Enum_TipoMedia.Imagen,
                            ".mp4" or ".webm" => Enum_TipoMedia.Video,
                            _ => Enum_TipoMedia.Imagen // por defecto
                        };

                        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                        var rutaGuardado = Path.Combine("wwwroot", "Media", "Ejercicios", ejercicio.Id.ToString());

                        if (!Directory.Exists(rutaGuardado))
                            Directory.CreateDirectory(rutaGuardado);

                        var rutaFinal = Path.Combine(rutaGuardado, nombreArchivo);

                        using (var stream = new FileStream(rutaFinal, FileMode.Create))
                        {
                            archivo.CopyTo(stream);
                        }

                        var media = new Media
                        {
                            Url = $"/Media/Ejercicios/{ejercicio.Id}/{nombreArchivo}",
                            Tipo = tipo,
                            Ejercicio = ejercicio
                        };

                        ejercicio.Medias.Add(media);
                    }
                }
            }

            rutinaServicio.GenerarNuevoEjercicio(ejercicio);
            return RedirectToAction("GestionEjercicios");
        }
        //Editar ejercicio
        [HttpGet]
        public IActionResult EditarEjercicio(int id)
        {
            var dto = rutinaServicio.ObtenerEjercicioDTOId(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        public IActionResult EditarEjercicio(EjercicioDTO dto, List<IFormFile> archivos)
        {
            Ejercicio ejercicio = rutinaServicio.ObtenerEjercicioId(dto.Id);
            if (ejercicio == null) return NotFound();

            // Actualizar campos
            ejercicio.Nombre = dto.Nombre;
            ejercicio.Tipo = dto.Tipo;
            ejercicio.GrupoMuscular = dto.GrupoMuscular;
            ejercicio.Instrucciones = dto.Instrucciones;

            // Agregar nuevas medias si hay archivos
            if (archivos != null && archivos.Count > 0)
            {
                foreach (var archivo in archivos)
                {
                    if (archivo.Length > 0)
                    {
                        var extension = Path.GetExtension(archivo.FileName).ToLower();
                        var tipo = extension switch
                        {
                            ".jpg" or ".jpeg" or ".png" => Enum_TipoMedia.Imagen,
                            ".mp4" or ".webm" => Enum_TipoMedia.Video,
                            _ => Enum_TipoMedia.Imagen
                        };

                        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                        var ruta = Path.Combine("wwwroot", "Media", "Ejercicios", ejercicio.Id.ToString());

                        if (!Directory.Exists(ruta))
                            Directory.CreateDirectory(ruta);

                        var rutaFinal = Path.Combine(ruta, nombreArchivo);
                        using (var stream = new FileStream(rutaFinal, FileMode.Create))
                        {
                            archivo.CopyTo(stream);
                        }

                        var media = new Media
                        {
                            Url = $"/Media/Ejercicios/{ejercicio.Id}/{nombreArchivo}",
                            Tipo = tipo,
                            EjercicioId = ejercicio.Id
                        };

                        ejercicio.Medias.Add(media);
                    }
                }
            }

            rutinaServicio.ModificarEjercicio(ejercicio);
            return RedirectToAction("GestionEjercicios");
        }

        [HttpGet]
        public IActionResult RegistrarRutina()
        {
            ViewBag.Ejercicios = rutinaServicio.ObtenerTodosEjercicios(); // EjercicioDTO
            ViewBag.Clientes = clienteServicio.ObtenerTodos(); // Cliente simple

            return View(new RutinaRegistroDTO());
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
                RutinaEjercicios = dto.IdsEjerciciosSeleccionados
                    .Select(id => new SesionRutina { EjercicioId = id }).ToList(),
                Asignados = dto.IdsClientesAsignados
                    .Select(id => clienteServicio.ObtenerPorId(id)).ToList()
            };

            rutinaServicio.GenerarNuevaRutina(rutina);
            return RedirectToAction("GestionRutinas");
        }
    }
    }


