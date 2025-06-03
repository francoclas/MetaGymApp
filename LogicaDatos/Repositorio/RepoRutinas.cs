using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using Microsoft.EntityFrameworkCore;

namespace LogicaDatos.Repositorio
{
    public class RepoRutinas : IRepositorioRutina
    {
        private readonly DbContextApp _context;

        public RepoRutinas (DbContextApp context)
        {
            _context = context;
        }

        public void Actualizar(Rutina entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        public void Agregar(Rutina entidad)
        {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public Rutina ObtenerPorId(int id)
        {
            return _context.Rutinas.FirstOrDefault(R => R.Id == id);
        }

        public List<Rutina> ObtenerRutinasCliente(int ClienteID)
        {
            throw new NotImplementedException();

        }

        public List<Rutina> ObtenerRutinasProfesional(int ProfesionalID)
        {
            throw new NotImplementedException();

        }

        public IEnumerable<Rutina> ObtenerTodos()
        {
            throw new NotImplementedException();
        }
    }
}
