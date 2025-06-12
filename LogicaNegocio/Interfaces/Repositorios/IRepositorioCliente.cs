using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaDatos.Interfaces.Repos
{
    public interface IRepositorioCliente : IRepository<Cliente>
    {
        bool ExisteUsuario(string usuario);
        
        Cliente VerificarCredenciales(string usuario,string pass);
        List<Cliente> BuscarClienteNombre(string Nombre);
        List<Cliente> BuscarClienteCI(string CI);
   

    }
}
