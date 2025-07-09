using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using Microsoft.AspNetCore.Http;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IMediaServicio
    {
        void AsignarMediaPorDefecto(Enum_TipoEntidad tipo, int idEntidad);
        Media GuardarArchivo(IFormFile archivo, Enum_TipoEntidad tipoEntidad, int idEntidad);
        void ReemplazarArchivo(IFormFile archivoNuevo, Enum_TipoEntidad tipoEntidad, int idEntidad);
        Media ObtenerMediaPorEntidad(Enum_TipoEntidad tipoEntidad, int idEntidad);
        void EliminarMedia(int mediaId);
        List<Media> ObtenerMediasPorEntidadGeneral(Enum_TipoEntidad tipo, int idEntidad);
        //Usuarios
        void AsignarFotoFavorita(int mediaId, Enum_TipoEntidad tipo, int entidadId);
        public Media? ObtenerFotoFavorita(Enum_TipoEntidad tipo, int idEntidad);
        List<Media> ObtenerImagenesUsuario(Enum_TipoEntidad tipo, int idEntidad);
        Media? ObtenerImagenPerfil(Enum_TipoEntidad tipo, int idEntidad);
    }
}
