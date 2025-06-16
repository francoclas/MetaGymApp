using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioCliente : IClienteServicio
    {
        private readonly IRepositorioCliente _repoCliente;
        public ServicioCliente(IRepositorioCliente repocli) {
            _repoCliente = repocli;
        }

        public List<Cliente> ObtenerTodos()
        {
            return _repoCliente.ObtenerTodos().ToList();
        }
    }
}
