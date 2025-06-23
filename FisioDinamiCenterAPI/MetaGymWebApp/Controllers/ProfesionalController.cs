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

        public ProfesionalController(IUsuarioServicio usuarioServicio, ICitaServicio citaServicio, IExtraServicio extraServicio, IProfesionalServicio proservicio, IRutinaServicio rutina, IMediaServicio mediaServicio)
        {
            this.usuarioServicio = usuarioServicio;
            this.citaServicio = citaServicio;
            this.extraServicio = extraServicio;
            this.profesionalServicio = proservicio;
            this.rutinaServicio = rutina;
            this.mediaServicio = mediaServicio;
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
            var rutinas = rutinaServicio.ObtenerPorProfesional(profesionalId);

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
        public IActionResult RegistrarEjercicio()
        {
            return View(new EjercicioDTO());
        }
        [HttpPost]
        [HttpPost]
        public IActionResult RegistrarEjercicio(EjercicioDTO dto, IFormFile archivo)
            {
            var nuevo = new Ejercicio
            {
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                GrupoMuscular = dto.GrupoMuscular,
                Instrucciones = dto.Instrucciones
            };

            rutinaServicio.GenerarNuevoEjercicio(nuevo);

            if (archivo != null && archivo.Length > 0)
            {
                mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Ejercicio, nuevo.Id);
            }

            TempData["Mensaje"] = "Ejercicio registrado correctamente.";
            return RedirectToAction("GestionRutinas");
        }
    }

}

