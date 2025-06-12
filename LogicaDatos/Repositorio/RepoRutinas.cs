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

        public void AsignarRutinaACliente(Cliente cliente, Rutina rutina)
        {
            if (!cliente.Rutinas.Contains(rutina))
            {
                cliente.Rutinas.Add(rutina);
                _context.SaveChanges();
            }
        }

        public List<Rutina> BuscarPorNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public bool ClienteTieneRutina(Cliente cliente, Rutina rutina)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public Rutina ObtenerPorId(int id)
        {
            return _context.Rutinas.FirstOrDefault(R => R.Id == id);
        }

        public List<Rutina> ObtenerPorProfesional(int profesionalId)
        {
            return _context.Rutinas
                    .Where(r => r.ProfesionalId == profesionalId)
                    .ToList();
        }

        public List<Rutina> ObtenerRutinasAsignadasACliente(int clienteId)
        {
            throw new NotImplementedException();
        }

        public List<SesionRutina> ObtenerSesionesPorCliente(int clienteId)
        {
            throw new NotImplementedException();
        }

        public SesionRutina? ObtenerSesionPorId(int sesionId)
        {
            throw new NotImplementedException();
        }

        public void RegistrarSesion(SesionRutina sesion)
        {
            throw new NotImplementedException();
        }

        public void RemoverRutinaDeCliente(Cliente cliente, Rutina rutina)
        {
            if (cliente.Rutinas.Contains(rutina))
            {
                cliente.Rutinas.Remove(rutina);
                _context.SaveChanges();
            }
        }
    }
    }
}
