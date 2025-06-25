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
    public class RepoEjercicios : IRepositorioEjercicio
    {
        private readonly DbContextApp _context;

        public RepoEjercicios (DbContextApp context)
        {
            _context = context;
        }

        public void Actualizar(Ejercicio entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        public void Agregar(Ejercicio entidad)
        {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        public List<Ejercicio> BuscarEjerciciosNombre(string Nombre)
        {
            return _context.Ejercicios
            .Include(e => e.Medias)
         .Where(e => e.Nombre.ToLower().Contains(Nombre.ToLower()))
         .ToList();
        }

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }
        public Ejercicio ObtenerPorId(int id)
        {
            return _context.Ejercicios
                    .Include(e => e.Medias)
                    .FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Ejercicio> ObtenerTodos()
        {
            return _context.Ejercicios
                    .Include(e => e.Medias)
                    .ToList();
        }
        public List<Ejercicio> BuscarPorGrupoMuscular(string grupoMuscular)
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .Where(e => e.GrupoMuscular.ToLower().Contains(grupoMuscular.ToLower()))
                .ToList();
        }

        public List<Ejercicio> BuscarPorTipo(string tipo)
        {
            return _context.Ejercicios
                .Include(e => e.Medias)
                .Where(e => e.Tipo.ToLower().Contains(tipo.ToLower()))
                .ToList();
        }

        public List<Ejercicio> ObtenerPorProfesional(int id)
        {
            return _context.Ejercicios.Include(e => e.Medias).Where(e => e.ProfesionalId == id).ToList();
        }
    }
}
