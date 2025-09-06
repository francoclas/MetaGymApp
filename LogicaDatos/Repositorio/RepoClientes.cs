using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Excepciones;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LogicaDatos.Repositorio
{
    // Repositorio EF Core para Cliente
    // Maneja persistencia y consultas específicas de clientes
    public class RepoClientes : IRepositorioCliente
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext
        public RepoClientes(DbContextApp context)
        {
            _context = context;
        }

        // Update de un cliente existente
        public void Actualizar(Cliente entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        // Guardar cambios sueltos (para operaciones múltiples)
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }

        // Alta de nuevo cliente
        public void Agregar(Cliente entidad)
        {
            _context.Clientes.Add(entidad);
            _context.SaveChanges();
        }

        // Buscar cliente por CI (usa Contains, aunque está filtrando por NombreUsuario en tu código)
        public List<Cliente> BuscarClienteCI(string CI)
        {
            return _context.Clientes
                .Where(c => c.NombreUsuario.ToLower().Contains(CI.ToLower()))
                .ToList();
        }

        // Buscar cliente por nombre (igual, sobre NombreUsuario)
        public List<Cliente> BuscarClienteNombre(string Nombre)
        {
            return _context.Clientes
                .Where(c => c.NombreUsuario.ToLower().Contains(Nombre.ToLower()))
                .ToList();
        }

        // Eliminar un cliente no se implementa 
        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        // Chequear existencia de usuario (case-insensitive)
        public bool ExisteUsuario(string usuario)
        {
            return _context.Clientes.Any(c => c.NombreUsuario.ToLower() == usuario.ToLower());
        }

        // Traer citas de un cliente entre fechas
        public List<Cita> ObtenerCitasEntreFechas(int ClienteID, DateTime FechaInicial, DateTime FechaFinal)
        {
            return _context.Citas
                .Where(c => c.ClienteId == ClienteID && c.FechaAsistencia >= FechaInicial && c.FechaAsistencia <= FechaFinal)
                .ToList();
        }

        // Citas de un cliente filtradas por estado
        public List<Cita> ObtenerCitasPorEstado(int ClienteID, EstadoCita Estado)
        {
            return _context.Citas
                .Where(c => c.Id == ClienteID && c.Estado == Estado)
                .ToList();
        }

        // Historial de citas de un cliente
        public List<Cita> ObtenerHistorial(int ClienteID)
        {
            return _context.Citas
                .Where(c => c.Id == ClienteID)
                .ToList();
        }

        // Cliente puntual con fotos de perfil
        public Cliente ObtenerPorId(int id)
        {
            return _context.Clientes
                .Include(p => p.FotosPerfil)
                .FirstOrDefault(C => C.Id == id);
        }

        // Buscar cliente por nombre de usuario
        public Cliente ObtenerPorUsuario(string usuario)
        {
            return _context.Clientes.SingleOrDefault(C => C.NombreUsuario == usuario);
        }

        // Listado completo de clientes
        public IEnumerable<Cliente> ObtenerTodos()
        {
            return _context.Clientes.ToList();
        }

        // Verificación de credenciales (sin hash, se compara texto plano)
        public Cliente VerificarCredenciales(string usuario, string pass)
        {
            return _context.Clientes.SingleOrDefault(u => u.NombreUsuario == usuario && u.Pass == pass);
        }

        // Chequear existencia de correo
        public bool ExisteCorreo(string correo)
        {
            return _context.Clientes.Any(c => c.Correo == correo);
        }

        // Chequear existencia de CI
        public bool ExisteCI(string cI)
        {
            return _context.Clientes.Any(c => (c.CI == cI));
        }
    }
}