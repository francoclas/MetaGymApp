using System.Collections.Generic;
using LogicaDatos;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;

public class RepoNotificacion : IRepositorioNotificacion
{
    private readonly DbContextApp _context;

    public RepoNotificacion(DbContextApp context)
    {
        _context = context;
    }

    public void Crear(Notificacion notificacion)
    {
        notificacion.Fecha = DateTime.Now;
        _context.Notificaciones.Add(notificacion);
        _context.SaveChanges();
    }

    public void MarcarComoLeida(int notificacionId)
    {
        var noti = _context.Notificaciones.FirstOrDefault(n => n.Id == notificacionId);
        if (noti != null)
        {
            noti.Leido = true;
            _context.SaveChanges();
        }
    }

    public List<Notificacion> ObtenerLeidasUsuario(int usuarioId, string rolUsuario)
    {
        List<Notificacion> salida = new List<Notificacion>();
        switch (rolUsuario.ToLower())
        {
            case "cliente":
                salida = _context.Notificaciones
                       .Where(n => n.ClienteId == usuarioId && n.Leido == true)
                       .ToList();
                break;

            case "profesional":
                salida = _context.Notificaciones
                       .Where(n => n.ProfesionalId == usuarioId && n.Leido == true)
                       .ToList();
                break;

            case "admin":
            case "administrador":
                salida = _context.Notificaciones
                       .Where(n => n.AdminId == usuarioId && n.Leido == true)
                       .ToList();
                break;

            default:
                return new List<Notificacion>();
        }
        return salida;
    }

    public List<Notificacion> ObtenerNoLeidasUsuario(int usuarioId, string rolUsuario)
    {
        List<Notificacion> salida = new List<Notificacion>();
        switch (rolUsuario.ToLower())
        {
            case "cliente":
                salida = _context.Notificaciones
                       .Where(n => n.ClienteId == usuarioId && n.Leido == false)
                       .ToList();
                break;

            case "profesional":
                salida = _context.Notificaciones
                       .Where(n => n.ProfesionalId == usuarioId && n.Leido == false)
                       .ToList();
                break;

            case "admin":
            case "administrador":
                salida = _context.Notificaciones
                       .Where(n => n.AdminId == usuarioId && n.Leido == false)
                       .ToList();
                break;

            default:
                return new List<Notificacion>();
        }
        return salida;
    }

    public List<Notificacion> ObtenerPorUsuario(int usuarioId, string rolUsuario, Enum_TipoNotificacion? tipo = null)
    {
        IQueryable<Notificacion> salida = _context.Notificaciones.AsQueryable();

        switch (rolUsuario.ToLower())
        {
            case "cliente":
                salida = salida.Where(n => n.ClienteId == usuarioId);
                break;

            case "profesional":
                salida = salida.Where(n => n.ProfesionalId == usuarioId);
                break;

            case "admin":
            case "administrador":
                salida = salida.Where(n => n.AdminId == usuarioId);
                break;

            default:
                return new List<Notificacion>();
        }

        if (tipo.HasValue)
        {
            salida = salida.Where(n => n.Tipo == tipo.Value);
        }

        return salida
            .OrderByDescending(n => n.Fecha)
            .ToList();
    }


}
