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
    // Maneja operaciones de persistencia y consultas específicas de profesionales
    public class RepoProfesional : IRepositorioProfesional
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext
        public RepoProfesional(DbContextApp context)
        {
            _context = context;
        }

        // Update de un profesional existente
        public void Actualizar(Profesional entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        // Guardar cambios genéricos
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }

        // Alta de nuevo profesional
        public void Agregar(Profesional entidad)
        {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        // Búsqueda por CI
        public List<Profesional> BuscarPorCi(string ci)
        {
            return _context.Profesionales
                .Include(p => p.Especialidades)
                .Include(p => p.TiposAtencion)
                .Include(p => p.Citas)
                .Include(p => p.Rutinas)
                .Include(p => p.Publicaciones)
                .Where(P => P.CI.ToLower().Contains(ci.ToLower()))
                .ToList();
        }

        // Búsqueda por nombre de usuario
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

        // Eliminar un profesional (no implementado)
        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        // Chequear existencia de usuario
        public bool ExisteUsuario(string usuario)
        {
            return _context.Profesionales.Any(p => p.NombreUsuario.ToLower() == usuario.ToLower());
        }

        // Profesional
        public Profesional ObtenerPorId(int id)
        {
            return _context.Profesionales
                .Include(p => p.Especialidades)
                .Include(p => p.TiposAtencion)
                .Include(p => p.Citas)
                .Include(p => p.Rutinas)
                .Include(p => p.FotosPerfil)
                .Include(p => p.Publicaciones)
                .Include(p => p.Agendas)
                .FirstOrDefault(C => C.Id == id);
        }

        // Buscar profesional por nombre de usuario
        public Profesional ObtenerPorUsuario(string usuario)
        {
            return _context.Profesionales.SingleOrDefault(P => P.NombreUsuario == usuario);
        }

        // Listado completo de profesionales con datos principales
        public IEnumerable<Profesional> ObtenerTodos()
        {
            return _context.Profesionales
                .Include(p => p.Especialidades)
                .Include(p => p.Citas)
                .Include(p => p.Rutinas)
                .Include(p => p.Publicaciones)
                .ToList();
        }

        // Verificación de credenciales
        public Profesional VerificarCredenciales(string usuario, string pass)
        {
            return _context.Profesionales.SingleOrDefault(P => P.NombreUsuario == usuario && P.Pass == pass);
        }

        // Chequear existencia de correo
        public bool ExisteCorreo(string correo)
        {
            return _context.Profesionales.Any(p => p.Correo == correo);
        }

        // Chequear existencia de CI
        public bool ExisteCI(string cI)
        {
            return _context.Profesionales.Any(p => p.CI == cI);
        }
    }
}