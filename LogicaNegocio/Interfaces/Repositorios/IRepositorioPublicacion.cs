using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Interfaces.Repositorios
{
    public interface IRepositorioPublicacion
    {
        List<Publicacion> ObtenerTodas();
        Publicacion ObtenerPorId(int id);
        void Crear(Publicacion publicacion);
        void ActualizarEstado(int idPublicacion, Enum_EstadoPublicacion nuevoEstado, int adminId, string? motivoRechazo);
        List<Publicacion> ObtenerPendientes();
        List<Publicacion> ObtenerAprobadasPublicas();
        List<Publicacion> ObtenerCreadasAdmin(int adminId);
        List<Publicacion> ObtenerAprobadasAdmin(int adminId);
        void Actualizar(Publicacion publicacion);
        List<Publicacion> ObtenerRechazadasAdmin(int adminId);
        bool UsuarioYaDioLike(int publicacionId, int usuarioId, string rol);
        void DarLike(int publicacionId, int usuarioId, string rol);
        void QuitarLike(int publicacionId, int usuarioId, string rol);
        int ContarLikes(int publicacionId);
    }
}
