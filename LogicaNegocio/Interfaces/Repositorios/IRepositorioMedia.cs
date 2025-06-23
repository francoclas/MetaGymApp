using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Interfaces.Repositorios
{
    public interface IRepositorioMedia
    {
        void Agregar(Media media);
        void Eliminar(Media media);
        Media ObtenerPorEntidad(Enum_TipoEntidad tipoEntidad, int idEntidad);
        Media ObtenerPorId(int mediaId);
        void Guardar();
    }
}
