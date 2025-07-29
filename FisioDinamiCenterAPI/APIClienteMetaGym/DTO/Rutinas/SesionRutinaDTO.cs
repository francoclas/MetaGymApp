namespace APIClienteMetaGym.DTO.Rutinas
{
    public class SesionRutinaDTO
    {
        public int RutinaId { get; set; }
        public int SesionRutinaId { get; set; }
        public int ClienteId { get; set; }
        public string NombreRutina { get; set; }
        public DateTime Fecha { get; set; }
        public int? DuracionMin { get; set; }
        public List<EjercicioRealizadoDTO> Ejercicios { get; set; } = new();
    }
}
