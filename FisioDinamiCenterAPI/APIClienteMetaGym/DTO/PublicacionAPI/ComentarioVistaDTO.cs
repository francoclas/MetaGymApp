using LogicaNegocio.Interfaces.DTOS;

namespace APIClienteMetaGym.DTO.PublicacionAPI
{
    public class ComentarioVistaDTO
    {
        public int PublicacionId { get; set; }
        public int? ComentarioPadreId { get; set; }
        public int ComentarioId { get; set; }
        public string Contenido { get; set; }
        public string Autor { get; set; }
        public string UrlImagenAutor { get; set; }
        public DateTime Fecha { get; set; }
        public int CantLikes { get; set; }
        public List<ComentarioVistaDTO> Respuestas { get; set; } = new();

    }
}
