using LogicaApp.DTOS;
using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    public class ProfesionalController : Controller
    {
        //Instancia
        private readonly IUsuarioServicio usuarioServicio;
        private readonly IProfesionalServicio profesionalServicio;
        private readonly ICitaServicio citaServicio;
        private readonly IExtraServicio extraServicio;
        private readonly IRutinaServicio rutinaServicio;

        public ProfesionalController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IProfesionalServicio proservicio, IRutinaServicio rutina)
        {
            this.usuarioServicio = usuarioServicio;
            this.citaServicio = citaServicio;
            this.extraServicio = extraServicio;
            this.profesionalServicio = proservicio;
            this.rutinaServicio = rutina;
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
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext); // Tu lógica

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
            var rutinas = rutinaServicio.ObtenerPorProfesional(profesionalId);

            return View(rutinas.Select(r => new RutinaDTO
            {
                Id = r.Id,
                NombreRutina = r.NombreRutina,
                Tipo = r.Tipo,
                UltimaModificacion = DateTime.Now // después podés usar r.FechaModif si lo agregás
            }).ToList());
        }
        //Ejercicios
        [HttpGet]
        public IActionResult RegistrarEjercicio()
        {
            return View(new EjercicioDTO());
        }
        [HttpPost]
        public IActionResult RegistrarEjercicio(EjercicioDTO dto, IFormFile archivo)
        {
            // Guardar el ejercicio primero
            var ejercicio = new Ejercicio { GrupoMuscular = dto.GrupoMuscular, Nombre = dto.Nombre,};

            // Si hay archivo subido
            if (archivo != null && archivo.Length > 0)
            {
                var nombreArchivo = $"{Guid.NewGuid()}_{archivo.FileName}";
                var ruta = Path.Combine("wwwroot", "Media", "Ejercicios", nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    archivo.CopyTo(stream);
                }

                var media = new Media
                {
                    Url = $"/Media/Ejercicios/{nombreArchivo}",
                    Tipo = Enum_TipoMedia.Imagen, // o Video si corresponde
                    EjercicioId = ejercicio.Id
                };
                ejercicio.Medias.Add(media);
                extraServicio.RegistrarMedia(media);
            }
            rutinaServicio.GenerarNuevoEjercicio(ejercicio);
            return RedirectToAction("GestionEjercicios");
        }
    }

}

