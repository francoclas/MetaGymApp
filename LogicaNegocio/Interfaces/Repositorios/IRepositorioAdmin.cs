using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaDatos.Interfaces.Repos
{
    public interface IRepositorioAdmin : IRepository<Admin>
    {
        bool ExisteUsuario(string usuario);
        Admin VerificarCredenciales(string usuario, string pass);

    }
}
