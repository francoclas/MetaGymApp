using LogicaNegocio.Interfaces.DTOS;

namespace MetaGymWebApp.Models
{
    public class MisPublicacionesProfesional
    {
        public List<PublicacionDTO> Pendientes { get; set; } = new();
        public List<PublicacionDTO> Aprobadas { get; set; } = new();
        public List<PublicacionDTO> Rechazadas { get; set; } = new();
    }
}
