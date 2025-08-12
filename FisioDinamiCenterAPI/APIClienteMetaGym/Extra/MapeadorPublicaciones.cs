using APIClienteMetaGym.DTO.PublicacionAPI;
using LogicaNegocio.Interfaces.DTOS;

namespace APIClienteMetaGym.Extra
{
    public class MapeadorPublicaciones
    {

        public List<PublicacionVistaDTO> MapearLista(List<PublicacionDTO> publicaciones)
        {
            return publicaciones.Select(p => new PublicacionVistaDTO
            {
                PublicacionId = p.Id,
                Titulo = p.Titulo,
                Descripcion = p.Descripcion,
                FechaCreacion = p.FechaCreacion,
                NombreAutor = p.NombreAutor,
                ImagenAutorURL = p.ImagenAutorURL,
                CantLikes = p.CantLikes,
                UrlsMedia = p.UrlsMedia,
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
                UrlImagenAutor = dto.ImagenAutor.Url,
                Fecha = dto.FechaCreacion,
                CantLikes = dto.CantLikes,
                Respuestas = dto.Respuestas?.Select(MapearComentarioRecursivo).ToList() ?? new()
            };
        }

    }

}
