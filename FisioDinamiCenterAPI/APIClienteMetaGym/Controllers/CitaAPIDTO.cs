using LogicaNegocio.Extra;

namespace APIClienteMetaGym.Controllers
{
    public class CitaAPIDTO
    {
        public int CitaId { get; set; }
        public int ClienteId { get; set; }
        public EstadoCita Estado { get; set; }
        public string Especialidad { get; set; }
        public string TipoAtencion { get; set; }
        public DateTime FechaAsistencia { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string NombreProfesional { get; set; }

    }
}