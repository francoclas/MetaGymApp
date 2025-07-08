using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;

namespace LogicaDatos.Repositorio
{
    public class RepoAdmin : IRepositorioAdmin
    {
        private readonly DbContextApp _context;

        public RepoAdmin (DbContextApp context)
        {
            _context = context;
        }
        public void Actualizar(Admin entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        public void Agregar(Admin entidad)
        {
            _context.Administradores.Add(entidad);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExisteUsuario(string usuario)
        {
            return _context.Administradores.Any(a => a.NombreUsuario.ToLower() == usuario.ToLower());
        }

        public void GuardarCambios()
        {
           _context.SaveChanges();
        }

        public Admin ObtenerPorId(int id)
        {
            return _context.Administradores.FirstOrDefault(A => A.Id == id);
        }

        public Admin ObtenerPorUsuario(string usuario)
        {
            return _context.Administradores.SingleOrDefault(A => A.NombreUsuario == usuario);
        }

        public IEnumerable<Admin> ObtenerTodos()
        {
            return _context.Administradores.ToList();
        }

        public Admin VerificarCredenciales(string usuario, string pass)
        {
            return _context.Administradores.SingleOrDefault(A => A.NombreUsuario == usuario && A.Pass == pass);
        }
    }
}
