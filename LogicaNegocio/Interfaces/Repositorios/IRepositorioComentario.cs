using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.Repositorios
{
    public interface IRepositorioComentario
    {
        List<Comentario> ObtenerPorPublicacion(int publicacionId);
        Comentario ObtenerPorId(int comentarioId);
        void Agregar(Comentario comentario);
        void ActualizarContenido(int comentarioId, string nuevoContenido);
        void Desactivar(int comentarioId);
        void IncrementarLikes(int comentarioId);
        void DecrementarLikes(int comentarioId);
    }
}
