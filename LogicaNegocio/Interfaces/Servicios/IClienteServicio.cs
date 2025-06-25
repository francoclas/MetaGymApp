using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IClienteServicio
    {
        Cliente ObtenerPorId(int id);

        //Datos personales

        List<Cliente> ObtenerTodos();
    }
}
