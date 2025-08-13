namespace APIClienteMetaGym.DTO
{
    public class ComentarioCrearDTO
    {
        public int PublicacionId { get; set; }
        public string Contenido { get; set; }
        public int? ComentarioPadreId { get; set; }
        public int AutorId { get; set; }
        public string RolAutor { get; set; } = "Cliente";
    }
}
