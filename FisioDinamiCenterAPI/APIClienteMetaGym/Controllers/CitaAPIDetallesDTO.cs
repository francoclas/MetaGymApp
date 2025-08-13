using APIClienteMetaGym.DTO.Rutinas;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;

namespace APIClienteMetaGym.Controllers
{
    public class CitaAPIDetallesDTO
    {
        public int CitaId { get; set; }
        public int ClienteId {  get; set; }
        public EstadoCita Estado { get; set; }
        public string Especialidad { get; set; }
        public string TipoAtencion { get; set; }
        public EstablecimientoAPIDTO Establecimiento { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAsistencia { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string NombreProfesional { get; set; }
        public string TelefonoProfesional { get; set; }
        public string? Conclusion { get; set; }
    }
}