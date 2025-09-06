using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{

    public class ServicioNotificacion : INotificacionServicio
    {
        // Repositorio de persistencia para Notificacion
        private readonly IRepositorioNotificacion _repositorioNotificacion;

        // Inyección de dependencias
        public ServicioNotificacion(IRepositorioNotificacion repo)
        {
            _repositorioNotificacion = repo;
        }

        // Notificaciones directas / personalizadas

        public void NotificacionPersonalizada(int usuarioId, string rol, Notificacion nueva)
        {
            _repositorioNotificacion.Crear(nueva);
            AsignarUsuario(nueva, usuarioId, rol);
        }

        // Notifica que se asignó una rutina a un cliente
        public void NotificarRutinaAsignada(int clienteId, int rutinaId)
        {
            var noti = new Notificacion
            {
                ClienteId = clienteId,
                Titulo = "Nueva rutina asignada",
                Mensaje = "Se te asignó una nueva rutina.",
                Tipo = Enum_TipoNotificacion.Rutina,
                RutinaId = rutinaId,
                Fecha = DateTime.Now,
                Leido = false
            };
            _repositorioNotificacion.Crear(noti);
        }

        // Notifica comentario en publicación (destinatario: autor de la publicación)
        public void NotificarComentario(int usuarioId, string rol, int publicacionId, string mensaje)
        {
            var noti = new Notificacion
            {
                Tipo = Enum_TipoNotificacion.Publicacion,
                Titulo = "Nuevo comentario",
                Mensaje = mensaje,
                PublicacionId = publicacionId,
                Fecha = DateTime.Now,
                Leido = false
            };

            // En este flujo primero se asigna y luego se crea (tal cual tu código)
            AsignarUsuario(noti, usuarioId, rol);
            _repositorioNotificacion.Crear(noti);
        }

        // Notifica respuesta a un comentario (destinatario: autor del comentario padre)
        public void NotificarInteraccionComentario(int usuarioId, string rol, int comentarioId, string mensaje)
        {
            var noti = new Notificacion
            {
                Tipo = Enum_TipoNotificacion.Comentario,
                Titulo = "Respuesta a tu comentario",
                Mensaje = mensaje,
                ComentarioId = comentarioId,
                Fecha = DateTime.Now,
                Leido = false
            };

            AsignarUsuario(noti, usuarioId, rol);
            _repositorioNotificacion.Crear(noti);
        }

        // Notifica cambio de estado de una cita
        public void NotificarCitaEstado(int usuarioId, string rol, int citaId, string nuevoEstado)
        {
            var noti = new Notificacion
            {
                Tipo = Enum_TipoNotificacion.Cita,
                Titulo = $"Cita {nuevoEstado}",
                Mensaje = $"Una de tus citas fue marcada como '{nuevoEstado}'.",
                CitaId = citaId,
                Fecha = DateTime.Now,
                Leido = false
            };

            AsignarUsuario(noti, usuarioId, rol);
            _repositorioNotificacion.Crear(noti);
        }

        // Setea el campo correspondiente (ClienteId/ProfesionalId/AdminId) según el rol
        private void AsignarUsuario(Notificacion n, int id, string rol)
        {
            switch (rol.ToLower())
            {
                case "cliente":
                    n.ClienteId = id;
                    break;
                case "profesional":
                    n.ProfesionalId = id;
                    break;
                case "admin":
                case "administrador":
                    n.AdminId = id;
                    break;
            }
        }

        // Marcado de lectura

        // Marca todas como leídas para un usuario/rol
        public void MarcarTodasComoLeidas(int usuarioId, string rol)
        {
            var notis = _repositorioNotificacion.ObtenerPorUsuario(usuarioId, rol);
            foreach (var n in notis.Where(n => !n.Leido))
            {
                _repositorioNotificacion.MarcarComoLeida(n.Id);
            }
        }

        // Marca una notificación específica como leída
        public void MarcarComoLeida(int notificacionId)
        {
            _repositorioNotificacion.MarcarComoLeida(notificacionId);
        }

        // Consultas (con mapeo a DTO)

        // Últimas N notificaciones del usuario/rol
        public List<NotificacionDTO> ObtenerUltimas(int usuarioId, string rol, int cantidad = 5)
        {
            var salida = _repositorioNotificacion.ObtenerPorUsuario(usuarioId, rol)
                .OrderByDescending(n => n.Fecha)
                .Take(cantidad)
                .ToList();

            return salida.Select(n => new NotificacionDTO
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leido = n.Leido,
                Tipo = n.Tipo,
                CitaId = n.CitaId,
                RutinaId = n.RutinaId,
                ComentarioId = n.ComentarioId,
                PublicacionId = n.PublicacionId
            }).ToList();
        }

        // Todas las notificaciones del usuario/rol (con posible filtro por tipo)
        public List<NotificacionDTO> ObtenerPorUsuario(int usuarioId, string rol, Enum_TipoNotificacion? tipo = null)
        {
            var notis = _repositorioNotificacion.ObtenerPorUsuario(usuarioId, rol, tipo);
            return notis.Select(n => new NotificacionDTO
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leido = n.Leido,
                Tipo = n.Tipo,
                CitaId = n.CitaId,
                RutinaId = n.RutinaId,
                ComentarioId = n.ComentarioId,
                PublicacionId = n.PublicacionId
            }).ToList();
        }

        // Conteo de no leídas para usuario/rol (opcional por tipo)
        public int ContarNoLeidas(int usuarioId, string rol, Enum_TipoNotificacion? tipo = null)
        {
            var notis = _repositorioNotificacion.ObtenerPorUsuario(usuarioId, rol, tipo);
            return notis.Count(n => !n.Leido);
        }

        // Solo leídas
        public List<NotificacionDTO> ObtenerLeidasUsuario(int usuarioId, string rolUsuario)
        {
            var salida = _repositorioNotificacion.ObtenerLeidasUsuario(usuarioId, rolUsuario)
                .OrderByDescending(n => n.Fecha).ToList();

            return salida.Select(n => new NotificacionDTO
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leido = n.Leido,
                Tipo = n.Tipo,
                CitaId = n.CitaId,
                RutinaId = n.RutinaId,
                ComentarioId = n.ComentarioId,
                PublicacionId = n.PublicacionId
            }).ToList();
        }

        // Solo no leídas
        public List<NotificacionDTO> ObtenerNoLeidasUsuario(int usuarioId, string rolUsuario)
        {
            var salida = _repositorioNotificacion.ObtenerNoLeidasUsuario(usuarioId, rolUsuario)
               .OrderByDescending(n => n.Fecha).ToList();

            return salida.Select(n => new NotificacionDTO
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leido = n.Leido,
                Tipo = n.Tipo,
                CitaId = n.CitaId,
                RutinaId = n.RutinaId,
                ComentarioId = n.ComentarioId,
                PublicacionId = n.PublicacionId
            }).ToList();
        }
    }
}
