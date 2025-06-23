using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.Repositorios
{
    public interface IRepositorioEjercicio : IRepository<Ejercicio>
    {
        public List<Ejercicio> BuscarEjerciciosNombre(string Nombre);
        public List<Ejercicio> BuscarPorGrupoMuscular(string GrupoMuscular);
        public List<Ejercicio> BuscarPorTipo(string Tipo);


    }
}
