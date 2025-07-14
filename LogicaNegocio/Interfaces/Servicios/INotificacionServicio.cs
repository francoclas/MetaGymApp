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
        void CrearNotificacionRutinaAsignada(int clienteId, int rutinaId);
        void CrearNotificacionComentario(int autorId, int publicacionId, string mensaje);
        void CrearNotificacionComentarioRespuesta(int autorComentarioId, int comentarioId, string mensaje);
        void CrearNotificacionCitaModificada(int usuarioId, int citaId, string nuevoEstado);
        void MarcarComoLeida(int notificacionId);
        List<NotificacionDTO> ObtenerPorUsuario(int usuarioId);
    }
}

