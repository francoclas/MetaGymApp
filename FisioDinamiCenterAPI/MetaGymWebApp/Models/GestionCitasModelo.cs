using LogicaApp.DTOS;
using LogicaNegocio.Extra;

namespace MetaGymWebApp.Models
{
    public class GestionCitasModelo
    {
        public List<CitaDTO> CitasEnEspera { get; set; }
        public List<CitaDTO> CitasProximasDeProfesional { get; set; }
        public List<CitaDTO> CitasAtendidasProfesional { get; set; }
        public EstadoCita EstadoSeleccionado { get; set; }
        public Boolean TieneEspecialidades { get; set; }
    }
}
