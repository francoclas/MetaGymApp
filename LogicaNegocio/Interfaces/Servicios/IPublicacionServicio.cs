using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
