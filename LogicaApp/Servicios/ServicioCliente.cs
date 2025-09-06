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
        private readonly IRepositorioCliente _repositorioCliente;
        public ServicioCliente(IRepositorioCliente repocli)
        {
            _repositorioCliente = repocli;
        }

        // Actualiza un cliente existente.
        public void ActualizarCliente(Cliente cliente)
        {
            _repositorioCliente.Actualizar(cliente);
        }

        // Trae un cliente por Id.
        public Cliente ObtenerPorId(int id)
        {
            return _repositorioCliente.ObtenerPorId(id);
        }

        // Devuelve todos los clientes en lista.
        public List<Cliente> ObtenerTodos()
        {
            return _repositorioCliente.ObtenerTodos().ToList();
        }

        // Mapea a DTO para listados/vistas de administrador.
        public List<ClienteDTO> ObtenerTodosDTO()
        {
            return _repositorioCliente.ObtenerTodos()
                .Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    Ci = c.CI,
                    NombreUsuario = c.NombreUsuario,
                    NombreCompleto = c.NombreCompleto,
                    Password = c.Pass,  
                    Correo = c.Correo,
                    Telefono = c.Telefono
                })
                .ToList();
        }
    }
}
