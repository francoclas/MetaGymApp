using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioCliente : IClienteServicio
    {
        private readonly IRepositorioCliente _repoCliente;
        public ServicioCliente(IRepositorioCliente repocli) {
            _repoCliente = repocli;
        }

        public void ActualizarCliente(Cliente cliente)
        {
            _repoCliente.Actualizar(cliente);
        }

        public Cliente ObtenerPorId(int id)
        {
            return _repoCliente.ObtenerPorId(id);
        }

        public List<Cliente> ObtenerTodos()
        {
            return _repoCliente.ObtenerTodos().ToList();
        }

        public List<ClienteDTO> ObtenerTodosDTO()
        {
            return _repoCliente.ObtenerTodos()
        .Select(c => new ClienteDTO
        {
            Id = c.Id,
            Ci = c.CI,
            NombreUsuario = c.NombreUsuario,
            NombreCompleto = c.NombreCompleto,
            Password = c.Pass,
            Correo = c.Correo,
            Telefono = c.Telefono
        }).ToList();
        }
    }
}
