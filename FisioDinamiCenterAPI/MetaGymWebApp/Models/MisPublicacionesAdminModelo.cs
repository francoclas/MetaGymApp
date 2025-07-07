using LogicaNegocio.Interfaces.DTOS;

namespace MetaGymWebApp.Models
{
    public class MisPublicacionesAdminModelo
    {
        public List<PublicacionDTO> PublicacionesCreadas { get; set; } = new();
        public List<PublicacionDTO> PublicacionesAutorizadas { get; set; } = new();
        public List<PublicacionDTO> PublicacionesRechazadas { get; set; } = new();
    }

}
