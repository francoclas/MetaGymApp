using APIClienteMetaGym.DTO.PublicacionAPI;
using LogicaNegocio.Interfaces.DTOS;

namespace APIClienteMetaGym.Extra
{
    public class MapeadorPublicaciones
    {
        private readonly string _baseUrl;

        public MapeadorPublicaciones(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public List<PublicacionVistaDTO> MapearLista(List<PublicacionDTO> publicaciones)
        {
            return publicaciones.Select(p => new PublicacionVistaDTO
            {
                PublicacionId = p.Id,
                Titulo = p.Titulo,
                Descripcion = p.Descripcion,
                FechaCreacion = p.FechaCreacion,
                NombreAutor = p.NombreAutor,
                ImagenAutorURL = CombinarUrl(p.ImagenAutorURL),
                CantLikes = p.CantLikes,
                UrlsMedia = p.UrlsMedia.Select(CombinarUrl).ToList(),
                Comentarios = MapearComentariosJerarquicos(p.Comentarios)
            }).ToList();
        }

        private List<ComentarioVistaDTO> MapearComentariosJerarquicos(List<ComentarioDTO> todos)
        {
            var comentariosRaiz = todos
                .Where(c => c.ComentarioPadreId == null)
                .OrderByDescending(c => c.FechaCreacion)
                .ToList();

            return comentariosRaiz.Select(MapearComentarioRecursivo).ToList();
        }

        private ComentarioVistaDTO MapearComentarioRecursivo(ComentarioDTO dto)
        {
            return new ComentarioVistaDTO
            {
               ComentarioId = dto.ComentarioId,
                Contenido = dto.Contenido,
                Autor = dto.AutorNombre,
                UrlImagenAutor = CombinarUrl(dto.ImagenAutor.Url),
                Fecha = dto.FechaCreacion,
                CantLikes = dto.CantLikes,
                Respuestas = dto.Respuestas?.Select(MapearComentarioRecursivo).ToList() ?? new()
            };
        }

        private string CombinarUrl(string? rutaRelativa)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa)) return string.Empty;
            return $"{_baseUrl}{(rutaRelativa.StartsWith("/") ? "" : "/")}{rutaRelativa}";
        }
    }

}
