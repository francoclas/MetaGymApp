namespace APIClienteMetaGym.DTO.PublicacionAPI
{
    public class PublicacionVistaDTO
    {
        public int PublicacionId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreAutor { get; set; }
        public string ImagenAutorURL { get; set; }
        public int CantLikes { get; set; }
        public List<string> UrlsMedia { get; set; } = new();
        public List<ComentarioVistaDTO> Comentarios { get; set; } = new();
    }
}
