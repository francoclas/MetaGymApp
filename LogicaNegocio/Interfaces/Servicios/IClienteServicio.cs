using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IClienteServicio
    {
        void ActualizarCliente(Cliente cliente);
        Cliente ObtenerPorId(int id);

        //Datos personales

        List<Cliente> ObtenerTodos();
        List<ClienteDTO> ObtenerTodosDTO();
    }
}
