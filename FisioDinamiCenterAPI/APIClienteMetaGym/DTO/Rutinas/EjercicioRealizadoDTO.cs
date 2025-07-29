namespace APIClienteMetaGym.DTO.Rutinas
{
    public class EjercicioRealizadoDTO
    {
        public int EjercicioId { get; set; }
        public string NombreEjercicio { get; set; }
        public bool SeRealizo { get; set; }
        public string? Observaciones { get; set; }
        public List<SerieRealizadaDTO> Series { get; set; } = new();
        public List<ValorMedicionDTO> Mediciones { get; set; } = new();
    }
}
