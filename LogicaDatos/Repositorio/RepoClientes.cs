using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Excepciones;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LogicaDatos.Repositorio
{
    public class RepoClientes : IRepositorioCliente
    {
        private readonly DbContextApp _context;
        public RepoClientes(DbContextApp context) {
            _context = context;
        }
        public void Actualizar(Cliente entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }
        public void Agregar(Cliente entidad)
        {
            _context.Clientes.Add(entidad);
                _context.SaveChanges();
        }

        public List<Cliente> BuscarClienteCI(string CI)
        {
            return _context.Clientes
            .Where(c => c.NombreUsuario.ToLower().Contains(CI.ToLower()))
            .ToList();
        }

        public List<Cliente> BuscarClienteNombre(string Nombre)
        {
            return _context.Clientes
            .Where(c => c.NombreUsuario.ToLower().Contains(Nombre.ToLower()))
            .ToList();
        }

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExisteUsuario(string usuario)
        {
            return _context.Clientes.Any(c => c.NombreUsuario.ToLower() == usuario.ToLower());
        }

        public List<Cita> ObtenerCitasEntreFechas(int ClienteID, DateTime FechaInicial, DateTime FechaFinal)
        {
            return _context.Citas
         .Where(c => c.ClienteId == ClienteID && c.FechaAsistencia >= FechaInicial && c.FechaAsistencia <= FechaFinal)
         .ToList();
        }

        public List<Cita> ObtenerCitasPorEstado(int ClienteID, EstadoCita Estado)
        {
            return _context.Citas
            .Where(c => c.Id == ClienteID && c.Estado == Estado)
            .ToList();
        }

        public List<Cita> ObtenerHistorial(int ClienteID)
        {
            return _context.Citas
            .Where(c => c.Id == ClienteID)
            .ToList();
        }
        public Cliente ObtenerPorId(int id)
        {
            return _context.Clientes.FirstOrDefault(C => C.Id == id);
        }

        public Cliente ObtenerPorUsuario(string usuario)
        {
            return _context.Clientes.SingleOrDefault(C => C.NombreUsuario == usuario);
        }

        public IEnumerable<Cliente> ObtenerTodos()
        {
            return _context.Clientes.ToList();
        }
        public Cliente VerificarCredenciales(string usuario, string pass)
        {
           return _context.Clientes.SingleOrDefault(u => u.NombreUsuario == usuario && u.Pass == pass);
        }
    }
}
