using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Repositorios;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LogicaDatos.Repositorio
{

    // Acá solo agrego comentarios y dejo tu lógica tal cual.
    public class RepoEjercicios : IRepositorioEjercicio
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext
        public RepoEjercicios(DbContextApp context)
        {
            _context = context;
        }

        // Update de un ejercicio existente
        public void Actualizar(Ejercicio entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        // Alta de un nuevo ejercicio
        public void Agregar(Ejercicio entidad)
        {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        // Búsqueda por nombre incluyendo medias
        public List<Ejercicio> BuscarEjerciciosNombre(string Nombre)
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .Where(e => e.Nombre.ToLower().Contains(Nombre.ToLower()))
                .ToList();
        }

        // Eliminación de un ejercicio
        public void Eliminar(int id)
        {
            Ejercicio ejercicio = _context.Ejercicios
                .Include(e => e.RutinaEjercicios)
                .FirstOrDefault(e => e.Id == id);

            if (ejercicio != null)
            {
                // Si está referenciado por rutinas, primero limpio esa tabla intermedia
                if (ejercicio.RutinaEjercicios.Any())
                    _context.RutinaEjercicios.RemoveRange(ejercicio.RutinaEjercicios);

                _context.Ejercicios.Remove(ejercicio);
                _context.SaveChanges();
            }
        }

        // Ejercicio puntual por Id (con medias y mediciones asociadas)
        public Ejercicio ObtenerPorId(int id)
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .Include(e => e.Mediciones)
                .FirstOrDefault(e => e.Id == id);
        }

        // Listado completo con medias
        public IEnumerable<Ejercicio> ObtenerTodos()
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .ToList();
        }

        // Búsqueda por grupo muscular con medias
        public List<Ejercicio> BuscarPorGrupoMuscular(string grupoMuscular)
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .Where(e => e.GrupoMuscular.ToLower().Contains(grupoMuscular.ToLower()))
                .ToList();
        }

        // Búsqueda por tipo con medias
        public List<Ejercicio> BuscarPorTipo(string tipo)
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .Where(e => e.Tipo.ToLower().Contains(tipo.ToLower()))
                .ToList();
        }

        // Todos los ejercicios creados por un profesional específico (con medias)
        public List<Ejercicio> ObtenerPorProfesional(int id)
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .Where(e => e.ProfesionalId == id)
                .ToList();
        }
    }
}