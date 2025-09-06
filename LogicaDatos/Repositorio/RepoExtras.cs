using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LogicaDatos.Repositorio
{
    // Especialidad, Establecimiento y TipoAtencion.
    public class RepoExtras : IRepositorioExtra
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext principal
        public RepoExtras(DbContextApp context)
        {
            _context = context;
        }

        // =======================
        // Altas
        // =======================

        // Alta de Especialidad
        public void AltaEspecialidad(Especialidad Nuevo)
        {
            _context.Especialidades.Add(Nuevo);
            _context.SaveChanges();
        }

        // Alta de Establecimiento
        public void AltaEstablecimiento(Establecimiento Nuevo)
        {
            _context.Establecimientos.Add(Nuevo);
            _context.SaveChanges();
        }

        // Alta de Media 
        public void AltaMedia(Media nueva)
        {
            _context.Medias.Add(nueva);
        }

        // =======================
        // Búsquedas por texto
        // =======================

        // Búsqueda de especialidades por nombre 
        public List<Especialidad> BuscarEspecialidad(string Nombre)
        {
            return _context.Especialidades
                .Where(e => e.NombreEspecialidad.ToLower().Contains(Nombre.ToLower()))
                .ToList();
        }

        // Búsqueda de establecimientos por nombre
        public List<Establecimiento> BuscarEstablecimiento(string Nombre)
        {
            return _context.Establecimientos
                .Where(e => e.Nombre.ToLower().Contains(Nombre.ToLower()))
                .ToList();
        }

        // =======================
        // Listados
        // =======================

        // Todas las especialidades con sus tipos de atención
        public List<Especialidad> ListarEspecialidades()
        {
            return _context.Especialidades
                .Include(e => e.TiposAtencion)
                .ToList();
        }

        // Todos los establecimientos con sus medias (fotos)
        public List<Establecimiento> ListarEstablecimientos()
        {
            return _context.Establecimientos
                .Include(e => e.Media)
                .ToList();
        }

        // =======================
        // Modificaciones
        // =======================

        // Modificar especialidad
        public void ModificarEspecialidad(Especialidad EspecialidadMod)
        {
            _context.Especialidades.Update(EspecialidadMod);
            _context.SaveChanges();
        }

        // Modificar establecimiento
        public void ModificarEstablecimiento(Establecimiento EstablecimientoMod)
        {
            _context.Establecimientos.Update(EstablecimientoMod);
            _context.SaveChanges();
        }

        // Especialidad puntual
        public Especialidad ObtenerEspecialidadId(int Id)
        {
            return _context.Especialidades
                .FirstOrDefault(E => E.Id == Id);
        }

        // Establecimiento puntual (con media)
        public Establecimiento ObtenerEstablecimientoId(int Id)
        {
            return _context.Establecimientos
                .Include(e => e.Media)
                .FirstOrDefault(E => E.Id == Id);
        }

        // Guardar cambios 
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }

        // =======================
        // Tipos de Atención
        // =======================

        // Alta de tipo de atención
        public void CrearTipoAtencion(TipoAtencion tipo)
        {
            _context.Add(tipo);
            _context.SaveChanges();
        }

        // Tipos por una especialidad puntual
        public List<TipoAtencion> ObtenerTiposAtencionPorEspecialidad(int especialidadId)
        {
            return _context.TipoAtenciones
                .Where(t => t.EspecialidadId == especialidadId)
                .ToList();
        }

        // Tipos para un conjunto de especialidades
        public List<TipoAtencion> ObtenerTiposAtencionPorEspecialidades(List<int> especialidadIds)
        {
            return _context.TipoAtenciones
                .Where(t => especialidadIds.Contains(t.EspecialidadId))
                .ToList();
        }

        // Un tipo puntual (por Id)
        public TipoAtencion ObtenerTipoPorId(int id)
        {
            return _context.TipoAtenciones.Find(id);
        }

        // Varios tipos por Ids
        public List<TipoAtencion> ObtenerTiposAtencionPorIds(List<int> ids)
        {
            return _context.TipoAtenciones
                .Where(t => ids.Contains(t.Id))
                .ToList();
        }

        // Todos los tipos con su especialidad cargada
        public List<TipoAtencion> ObtenerTiposAtencionTodos()
        {
            return _context.TipoAtenciones
                .Include(t => t.Especialidad)
                .ToList();
        }

        // Tipos de atención que pertenecen a las especialidades de un profesional
        public List<TipoAtencion> ObtenerTiposAtencionPorProfesional(int profesionalId)
        {
            return _context.TipoAtenciones
                .Include(t => t.Especialidad)
                .Where(t => t.Especialidad.Profesionales.Any(p => p.Id == profesionalId))
                .ToList();
        }

        // Un tipo puntual (por Id)
        public TipoAtencion ObtenerTipoAtencionId(int? tipoAtencionId)
        {
            return _context.TipoAtenciones
                .Include(t => t.Especialidad)
                .FirstOrDefault(ta => ta.Id == tipoAtencionId);
        }
    }
}