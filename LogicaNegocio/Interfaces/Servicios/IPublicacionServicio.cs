using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IPublicacionServicio
    {
        //ABM
        void CrearPublicacion(CrearPublicacionDTO dto);
        void CrearPublicacionAdmin(Publicacion publicacion);
        void ModerarPublicacion(ModerarPublicacionDTO dto);
        void CrearPublicacionImagenes(Publicacion publicacion);
        void ActualizarPublicacion(PublicacionDTO pub);
        //Listar
        List<PublicacionDTO> ObtenerPublicaciones();
        PublicacionDTO ObtenerPorId(int id);
        List<PublicacionDTO> ObtenerPendientes();
        List<PublicacionDTO> ObtenerTodas();
        List<PublicacionDTO> ObtenerPorProfesionalId(int profesionalId);
        List<PublicacionDTO> ObtenerCreadasPorAdmin(int adminId);
        List<PublicacionDTO> ObtenerAutorizadasPorAdmin(int adminId);
        void AprobarPublicacion(int publicacionId, int v);
        void RechazarPublicacion(int publicacionId, string motivoRechazo, int v);
        List<PublicacionDTO> ObtenerRechazadasPorAdmin(int adminId);
        List<PublicacionDTO> ObtenerPublicacionesInicio();
        List<PublicacionDTO> ObtenerPublicacionesInicioAPI();
        List<PublicacionDTO> ObtenerNovedades();
        //Interacciones
        void DarLikePublicacion(int publicacionId, int usuarioId, string rol);
        void QuitarLikePublicacion(int publicacionId, int usuarioId, string rol);
        bool UsuarioYaDioLikePublicacion(int publicacionId, int usuarioId, string rol);
        int ContarLikesPublicacion(int publicacionId);
        void OcultarComentario(int comentarioId);
    }
}
