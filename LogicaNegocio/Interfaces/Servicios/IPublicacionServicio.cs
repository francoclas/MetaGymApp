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
        List<PublicacionDTO> ObtenerPublicaciones();
        PublicacionDTO ObtenerPorId(int id);
        void CrearPublicacion(CrearPublicacionDTO dto);
        void ModerarPublicacion(ModerarPublicacionDTO dto);
        List<PublicacionDTO> ObtenerPendientes();
        List<PublicacionDTO> ObtenerTodas();
        List<PublicacionDTO> ObtenerPorProfesionalId(int profesionalId);
        void CrearPublicacionImagenes(Publicacion publicacion);
        List<PublicacionDTO> ObtenerCreadasPorAdmin(int adminId);
        List<PublicacionDTO> ObtenerAutorizadasPorAdmin(int adminId);
        void AprobarPublicacion(int publicacionId, int v);
        void RechazarPublicacion(int publicacionId, string motivoRechazo, int v);
        List<PublicacionDTO> ObtenerRechazadasPorAdmin(int adminId);
    }
}
