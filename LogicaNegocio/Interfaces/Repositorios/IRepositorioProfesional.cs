using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaDatos.Interfaces.Repos
{
    public interface IRepositorioProfesional : IRepository<Profesional>
    {
        bool ExisteUsuario(string usuario);
        Profesional VerificarCredenciales(string usuario, string pass);
        List<Profesional> BuscarPorCi(string ci);
        List<Profesional> BuscarPorNombre(string Nombre);
        Profesional ObtenerPorUsuario(string usuario);
        void GuardarCambios();
    }
}
