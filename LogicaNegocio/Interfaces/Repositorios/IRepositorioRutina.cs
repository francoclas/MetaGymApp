using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaDatos.Interfaces.Repos
{
    public interface IRepositorioRutina : IRepository<Rutina>
    {
        List<Rutina> ObtenerRutinasCliente(int ClienteID);
        List<Rutina> ObtenerRutinasProfesional(int ProfesionalID);

    }
}
