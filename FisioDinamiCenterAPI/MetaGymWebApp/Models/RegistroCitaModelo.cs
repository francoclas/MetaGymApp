using LogicaApp.DTOS;
using LogicaNegocio.Interfaces.DTOS;

namespace MetaGymWebApp.Models
{
    public class RegistroCitaModelo
    {
        public List<ClienteDTO> Clientes { get; set; }
        public List<EspecialidadDTO> Especialidades { get; set; }
        public List<TipoAtencionDTO> TiposAtencion { get; set; }
        public List<EstablecimientoDTO> Establecimientos { get; set; }
        public CitaDTO Cita { get; set; }
    }
}
