using LogicaNegocio.Interfaces.DTOS;

namespace MetaGymWebApp.Models
{
    //Se utiliza para mostrar los ejercicios en vista "GestionEjercicios" en el profesional.
    public class GestionEjerciciosModelo
    {
        public List<EjercicioDTO> EjerciciosProfesional { get; set; }
        public List<EjercicioDTO> EjerciciosSistema { get; set; }
    }
}
