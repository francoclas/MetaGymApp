using APIClienteMetaGym.Controllers;
using APIClienteMetaGym.DTO.Rutinas;
using LogicaApp.DTOS;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS.API;

namespace APIClienteMetaGym.Extra
{
    public class MapeadorCitas
    {
        public CitaAPIDTO MapearCitaAPIDTO(CitaDTO cita)
        {
            return new CitaAPIDTO
            {
                CitaId = cita.CitaId,
                ClienteId = cita.ClienteId,
                Estado = cita.Estado,
                Especialidad = cita.Especialidad?.NombreEspecialidad,
                FechaAsistencia = cita.FechaAsistencia,
                FechaCreacion = cita.FechaCreacion,
                FechaFinalizacion = cita.FechaFinalizacion,
                NombreProfesional = cita.NombreProfesional
            };
        }

        public CitaAPIDetallesDTO MapearCitaAPIDetallesDTO(CitaDTO cita)
        {
            return new CitaAPIDetallesDTO
            {
                CitaId = cita.CitaId,
                ClienteId = cita.ClienteId,
                Estado = cita.Estado,
                Especialidad = cita.Especialidad?.NombreEspecialidad,
                TipoAtencion = cita.TipoAtencion?.Nombre,
                Establecimiento = MapearEstablecimientoAPIDTO(cita.Establecimiento),
                Descripcion = cita.Descripcion,
                FechaAsistencia = cita.FechaAsistencia,
                FechaCreacion = cita.FechaCreacion,
                FechaFinalizacion = cita.FechaFinalizacion,
                NombreProfesional = cita.NombreProfesional,
                TelefonoProfesional = cita.TelefonoProfesional,
                Conclusion = cita.Conclusion
            };
        }

        private EstablecimientoAPIDTO MapearEstablecimientoAPIDTO(Establecimiento est)
        {
            if (est == null) return null;

            return new EstablecimientoAPIDTO
            {
                Id = est.Id,
                Nombre = est.Nombre,
                Direccion = est.Direccion,
                Latitud = est.Latitud,
                Longitud = est.Longitud,
                UrlMedia = est.Media?.FirstOrDefault()?.Url
            };
        }

        public List<object> MapearEstadosCita()
        {
            return Enum.GetValues(typeof(EstadoCita))
                .Cast<EstadoCita>()
                .Select(e => new
                {
                    Id = (int)e,
                    Estado = e.ToString()
                })
                .ToList<object>();
        }
    }
}
