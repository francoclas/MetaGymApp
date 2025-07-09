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
    public class RepoProfesional : IRepositorioProfesional
    {
        private readonly DbContextApp _context;
        
        public RepoProfesional(DbContextApp context)
        {
            _context = context;
        }
        public void Actualizar(Profesional entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }
        public void Agregar(Profesional entidad)
            {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        public List<Profesional> BuscarPorCi(string ci)
        {
            return _context.Profesionales
            .Include(p => p.Especialidades)
            .Include(p => p.Citas)
            .Include(p => p.Rutinas)
            .Include(p => p.Publicaciones)
            .Where(P => P.CI.ToLower().Contains(ci.ToLower()))
            .ToList();
        }

        public List<Profesional> BuscarPorNombre(string Nombre)
        {
            return _context.Profesionales
                .Include(p => p.Especialidades)
                .Include(p => p.Citas)
                .Include(p => p.Rutinas)
                .Include(p => p.Publicaciones)
                    .Where(c => c.NombreUsuario.ToLower().Contains(Nombre.ToLower()))
                    .ToList();
                }

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExisteUsuario(string usuario)
        {
            return _context.Profesionales.Any(p => p.NombreUsuario.ToLower() == usuario.ToLower());
        }

        public Profesional ObtenerPorId(int id)
        {
            return _context.Profesionales
                .Include(p => p.Especialidades)
                .Include(p => p.Citas)
                .Include(p => p.Rutinas)
                .Include(p => p.FotosPerfil)
                .Include(p => p.Publicaciones)
                .FirstOrDefault(C => C.Id == id)

                ;
        }

        public Profesional ObtenerPorUsuario(string usuario)
        {
            return _context.Profesionales.SingleOrDefault(P => P.NombreUsuario == usuario);
        }

        public IEnumerable<Profesional> ObtenerTodos()
        {
            return _context.Profesionales
                .Include(p => p.Especialidades)
                .Include(p => p.Citas)
                .Include(p => p.Rutinas)
                .Include(p => p.Publicaciones)
                .ToList();
        }

        public Profesional VerificarCredenciales(string usuario, string pass)
        {
            return _context.Profesionales.SingleOrDefault(P => P.NombreUsuario == usuario && P.Pass == pass);
        }
    }
}
