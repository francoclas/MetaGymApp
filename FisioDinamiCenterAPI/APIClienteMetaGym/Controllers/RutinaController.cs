using APIClienteMetaGym.DTO;
using APIClienteMetaGym.DTO.Rutinas;
using APIClienteMetaGym.Extra;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIClienteMetaGym.Controllers
{
    [ApiController]
    [Route("api/rutinas")]
    [Authorize]
    public class RutinaController : Controller
    {
        private readonly IRutinaServicio rutinaServicio;
        private readonly IClienteServicio clienteServicio;
        public RutinaController(IRutinaServicio rs, IClienteServicio cs)
        {
            rutinaServicio = rs;
            clienteServicio = cs;
        }
        /// <summary>
        /// Obtiene las rutinas asignadas a un cliente.
        /// </summary>
        [HttpGet("asignadas")]
        public IActionResult ObtenerRutinasAsignadas([FromQuery] int clienteId)
        {
            try
            {
                Cliente aux = clienteServicio.ObtenerPorId(clienteId);
            }
            catch (Exception e)
            {
                return BadRequest(RespuestaApi<string>.Unauthorized("El cliente no existe"));
            }
            List<RutinaAsignada> rutinas = rutinaServicio.ObtenerRutinasAsignadasCliente(clienteId);
            return Ok(RespuestaApi<List<RutinaAsignada>>.Ok(rutinas));
        }
        /// <summary>
        /// Obtiene la informacion de los ejercicios de una rutina en particular.
        /// </summary>
        [HttpGet("informacionRutina")]
        public IActionResult ObtenerInformacionRutina([FromQuery] int rutinaId)
        {
            try
            {
                Rutina rutina = rutinaServicio.ObtenerRutinaPorId(rutinaId);
                if(rutina == null)
                    return NotFound(RespuestaApi<string>.NotFound("La rutina no existe."));
                return Ok(RespuestaApi<RutinaDTO>.Ok(new MapeadorRutinas().MapearRutinaDTO(rutina)));
            }
            catch (Exception e)
            {
                return BadRequest(RespuestaApi<string>.Error(e.Message));
            }
        }
        /// <summary>
        /// Obtiene la información de un ejercicio por ID.
        /// </summary>
        [HttpGet("ejercicio/{id}")]
        public IActionResult ObtenerEjercicio(int id)
        {
            var ejercicio = rutinaServicio.ObtenerEjercicioDTOId(id);
            if (ejercicio == null)
                return NotFound(RespuestaApi<string>.Error("Ejercicio no encontrado."));

            return Ok(RespuestaApi<EjercicioVistaDTO>.Ok(MapearEjercicioDTO(ejercicio)));
        }
        /// <summary>
        /// Registra una nueva sesión de entrenamiento para una rutina asignada.
        /// </summary>
        [HttpPost("sesion")]
        public IActionResult RegistrarSesion([FromBody] SesionRutinaDTO sesion)
        {
            SesionRutina nueva = MapearSesionRutinaNueva(sesion);
            try
            {
                SesionRutina registrada = rutinaServicio.RegistrarSesion(nueva);
                return Ok(RespuestaApi<SesionRutinaDTO>.Ok(MapearSesionRutina(registrada)));
            }
            catch (Exception e)
            {
                return BadRequest(RespuestaApi<string>.Error(e.Message));
            }
        }
        /// <summary>
        /// Obtiene el historial de sesiones del cliente.
        /// </summary>
        [HttpGet("historial")]
        public IActionResult HistorialCliente([FromQuery] int clienteId)
        {
            try
            {
                //valido cliente
                Cliente aux = clienteServicio.ObtenerPorId(clienteId);
                if (aux == null)
                    throw new Exception("El usuario no se encontro, pruebe volviendo a iniciar sesion.");
            }
            catch (Exception e)
            {
                return BadRequest(RespuestaApi<string>.Forbidden(e.Message));
            }
            var sesiones = rutinaServicio.ObtenerHistorialClienteDTO(clienteId);
            return Ok(RespuestaApi<List<SesionEntrenadaDTO>>.Ok(sesiones));
        }
        /// <summary>
        /// Obtiene el historial de sesiones del cliente.
        /// </summary>
        [HttpGet("sesionEntrenamiento")]
        public IActionResult sesionEntrenamiento([FromQuery] int sesionId)
        {
            try
            {
                SesionEntrenadaDTO sesiones = rutinaServicio.ObtenerSesionEntrenamiento(sesionId);
                return Ok(RespuestaApi<SesionEntrenadaDTO>.Ok(sesiones));

            }
            catch (Exception e)
            {
                return BadRequest(RespuestaApi<string>.Error("No se encontro sesion de entrenamiento."));
            }
        }
        //Mapeos
        private EjercicioRealizadoDTOAPI MapearEjercicioRealizado(EjercicioRealizado er)
        {
            return new EjercicioRealizadoDTOAPI
            {
                NombreEjercicio = er.Ejercicio?.Nombre ?? "Ejercicio desconocido",
                SeRealizo = er.SeRealizo,
                Observaciones = er.Observaciones,
                Series = er.Series.Select(s => new SerieRealizadaDTO
                {
                    Repeticiones = s.Repeticiones,
                    PesoUtilizado = s.PesoUtilizado
                }).ToList(),
                Mediciones = er.ValoresMediciones.Select(vm => new ValorMedicionDTO
                {
                    MedicionId = vm.Id,
                    NombreMedicion = vm.Medicion.Nombre,
                    Descripcion = vm.Medicion.Descripcion,
                    Unidad = vm.Medicion.Unidad
                }).ToList()
            };
        }
        private SesionRutina MapearSesionRutinaNueva(SesionRutinaDTO sesion)
        {
            return new SesionRutina
            {
                RutinaAsignadaId = sesion.RutinaId,
                ClienteId = sesion.ClienteId,
                FechaRealizada = sesion.Fecha,
                DuracionMin = sesion.DuracionMin,
                EjerciciosRealizados = sesion.Ejercicios.Select(e => new EjercicioRealizado
                {
                    EjercicioId = e.EjercicioId,
                    SeRealizo = e.SeRealizo,
                    Observaciones = e.Observaciones,
                    Series = e.Series.Select(s => new SerieRealizada
                    {
                        Repeticiones = s.Repeticiones,
                        PesoUtilizado = s.PesoUtilizado
                    }).ToList(),
                    ValoresMediciones = e.Mediciones.Select(m => new ValorMedicion
                    {
                        MedicionId = m.MedicionId,
                        Valor = m.Valor
                    }).ToList()
                }).ToList()
            };
        }
        private SesionRutinaDTO MapearSesionRutina(SesionRutina sesion)
        {
            return new SesionRutinaDTO
            {
                SesionRutinaId = sesion.Id,
                NombreRutina = sesion.NombreRutinaHistorial,
                Fecha = sesion.FechaRealizada,
                DuracionMin = sesion.DuracionMin,
                Ejercicios = sesion.EjerciciosRealizados?.Select(MapearEjercicioRealizado).ToList() ?? new()
            };
        }
        private static EjercicioVistaDTO MapearEjercicioDTO(EjercicioDTO dto)
        {
            return new EjercicioVistaDTO
            {
                Id = dto.Id,
                ProfesionalId = dto.ProfesionalId,
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                GrupoMuscular = dto.GrupoMuscular,
                Instrucciones = dto.Instrucciones,
                UrlMedia = dto.Media?.Url,
                Medias = dto.Medias?.Select(m => m.Url).ToList() ?? new List<string>(),
                Mediciones = dto.Mediciones?.Select(m => new ValorMedicionDTO
                {
                    MedicionId = m.Id,
                    NombreMedicion = m.Nombre, 
                    Descripcion = m.Descripcion,
                    Unidad = m.Unidad
                }).ToList() ?? new List<ValorMedicionDTO>()
            };
        }
    }
}
