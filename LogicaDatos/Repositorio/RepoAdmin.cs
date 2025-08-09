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

        public bool ExisteCI(string cI)
        {
            return _context.Administradores.Any(a => a.CI == cI);
        }

        public bool ExisteCorreo(string correo)
        {
            return _context.Administradores.Any(a => a.Correo == correo);   
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
            return _context.Administradores
                .Include(p => p.FotosPerfil)
                .FirstOrDefault(A => A.Id == id);
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
