using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface INotificacionServicio
    {
            void NotificacionPersonalizada(int usuarioId, string rol, Notificacion nueva);
            void NotificarRutinaAsignada(int clienteId, int rutinaId);
            void NotificarComentario(int usuarioId, string rol, int publicacionId, string mensaje);
            void NotificarInteraccionComentario(int usuarioId, string rol, int comentarioId, string mensaje);
            void NotificarCitaEstado(int usuarioId, string rol, int citaId, string nuevoEstado);
            void MarcarComoLeida(int notificacionId);
            List<NotificacionDTO> ObtenerPorUsuario(int usuarioId, string rol, Enum_TipoNotificacion? tipo = null);
            List<NotificacionDTO> ObtenerLeidasUsuario(int usuarioId, string rolUsuario);
            List<NotificacionDTO> ObtenerNoLeidasUsuario(int usuarioId, string rolUsuario);
            public List<NotificacionDTO> ObtenerUltimas(int usuarioId, string rol, int cantidad = 5);
        public int ContarNoLeidas(int usuarioId, string rol, Enum_TipoNotificacion? tipo = null);
            public void MarcarTodasComoLeidas(int usuarioId, string rol);

        }

    }


