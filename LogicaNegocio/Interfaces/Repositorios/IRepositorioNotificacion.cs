using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.Repositorios
{
    public interface IRepositorioNotificacion
    {
        void Crear(Notificacion notificacion);
        void MarcarComoLeida(int notificacionId);
        List<Notificacion> ObtenerPorUsuario(int usuarioId, string rolUsuario, Enum_TipoNotificacion? tipo = null);

        List<Notificacion> ObtenerLeidasUsuario(int usuarioId, string rolUsuario);
        List<Notificacion> ObtenerNoLeidasUsuario(int usuarioId, string rolUsuario);
    }
}
