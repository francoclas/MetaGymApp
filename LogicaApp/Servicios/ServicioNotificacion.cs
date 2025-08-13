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
        private readonly IRepositorioNotificacion _repo;

        public ServicioNotificacion(IRepositorioNotificacion repo)
        {
            _repo = repo;
        }
        public void NotificacionPersonalizada(int usuarioId, string rol, Notificacion nueva)
        {
            _repo.Crear(nueva);
            AsignarUsuario(nueva, usuarioId,rol);
        }

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
            _repo.Crear(noti);
        }

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

            AsignarUsuario(noti, usuarioId, rol);
            _repo.Crear(noti);
        }

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
            _repo.Crear(noti);
        }

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
            _repo.Crear(noti);
        }
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
        public void MarcarTodasComoLeidas(int usuarioId, string rol)
        {
            var notis = _repo.ObtenerPorUsuario(usuarioId, rol);
            foreach (var n in notis.Where(n => !n.Leido))
            {
                _repo.MarcarComoLeida(n.Id);
            }
        }
        public void MarcarComoLeida(int notificacionId)
        {
            _repo.MarcarComoLeida(notificacionId);
        }

        public List<NotificacionDTO> ObtenerUltimas(int usuarioId, string rol, int cantidad = 5)
        {
            var salida = _repo.ObtenerPorUsuario(usuarioId, rol)
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
        public List<NotificacionDTO> ObtenerPorUsuario(int usuarioId, string rol, Enum_TipoNotificacion? tipo = null)
        {
            var notis = _repo.ObtenerPorUsuario(usuarioId, rol, tipo);
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

        public int ContarNoLeidas(int usuarioId, string rol, Enum_TipoNotificacion? tipo = null)
        {
            var notis = _repo.ObtenerPorUsuario(usuarioId, rol, tipo);
            return notis.Count(n => !n.Leido);
        }

        public List<NotificacionDTO> ObtenerLeidasUsuario(int usuarioId, string rolUsuario)
        {
            var salida = _repo.ObtenerLeidasUsuario(usuarioId, rolUsuario)
                .OrderByDescending(n => n.Fecha).ToList();

            return salida.Select(n => new NotificacionDTO {
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

        public List<NotificacionDTO> ObtenerNoLeidasUsuario(int usuarioId, string rolUsuario)
        {
            var salida = _repo.ObtenerNoLeidasUsuario(usuarioId, rolUsuario)
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
