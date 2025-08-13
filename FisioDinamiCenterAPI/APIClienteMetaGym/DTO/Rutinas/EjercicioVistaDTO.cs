using LogicaNegocio.Clases;

namespace APIClienteMetaGym.DTO.Rutinas
{
    public class EjercicioVistaDTO
    {
        public int Id { get; set; }
        public int ProfesionalId { get; set; }

        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string GrupoMuscular { get; set; }
        public string Instrucciones { get; set; }
        public string UrlMedia { get; set; }
        public List<string> Medias { get; set; } = new List<string>();
        public List<ValorMedicionDTO> Mediciones { get; set; } = new List<ValorMedicionDTO>();
    }
}
