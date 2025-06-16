using LogicaApp.DTOS;
using LogicaNegocio.Extra;

namespace MetaGymWebApp.Models
{
    public class GestionCitasModelo
    {
        public List<CitaDTO> Citas { get; set; }
        public EstadoCita EstadoSeleccionado { get; set; }
    }
}
