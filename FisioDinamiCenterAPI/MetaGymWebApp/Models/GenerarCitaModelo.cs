using LogicaApp.DTOS;
using LogicaNegocio.Interfaces.DTOS;

namespace MetaGymWebApp.Models
{
    public class GenerarCitaModelo
    {
        public CitaDTO Cita { get; set; } = new CitaDTO();
        public List<EspecialidadDTO> Especialidades { get; set; } = new();
        public List<EstablecimientoDTO> Establecimientos { get; set; } = new();
        public List<TipoAtencionDTO> TiposAtencion { get; set; } = new();
    }
}
