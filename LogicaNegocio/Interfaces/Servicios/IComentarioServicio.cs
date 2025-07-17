using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Interfaces.DTOS;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IComentarioServicio
    {
            List<ComentarioDTO> ObtenerPorPublicacion(int publicacionId);
            void AgregarComentario(ComentarioDTO dto);
            void EditarComentario(int comentarioId, string nuevoContenido);
            void EliminarComentario(int comentarioId);
            void DarLikeComentario(int comentarioId, int usuarioId, string rol);
            void QuitarLikeComentario(int comentarioId, int usuarioId, string rol);
            bool UsuarioYaDioLikeComentario(int comentarioId, int usuarioId, string rol);
            public int ContarLikesComentario(int id);
    }
}
