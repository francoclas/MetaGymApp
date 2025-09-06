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
    // Repositorio EF Core para Admin
    // Acceso CRUD simple usando DbContextApp
    public class RepoAdmin : IRepositorioAdmin
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext
        public RepoAdmin(DbContextApp context)
        {
            _context = context;
        }

        // Update de un admin ya trackeado o adjuntado
        public void Actualizar(Admin entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        // Alta de un admin
        public void Agregar(Admin entidad)
        {
            _context.Administradores.Add(entidad);
            _context.SaveChanges();
        }

        // Borrado por id (sin implementar)
        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        // Checks de existencia para validaciones previas
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

        // Guardar cambios 
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }

        // Traer admin por Id, incluyendo fotos de perfil (navegación)
        public Admin ObtenerPorId(int id)
        {
            return _context.Administradores
                .Include(p => p.FotosPerfil)
                .FirstOrDefault(A => A.Id == id);
        }

        // Login: búsqueda directa por nombre de usuario
        public Admin ObtenerPorUsuario(string usuario)
        {
            return _context.Administradores.SingleOrDefault(A => A.NombreUsuario == usuario);
        }

        // Lista completa 
        public IEnumerable<Admin> ObtenerTodos()
        {
            return _context.Administradores.ToList();
        }

        // Verificación de credenciales (hash/seguridad se resuelve fuera)
        public Admin VerificarCredenciales(string usuario, string pass)
        {
            return _context.Administradores.SingleOrDefault(A => A.NombreUsuario == usuario && A.Pass == pass);
        }
    }
}