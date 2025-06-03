using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IExtraServicio
    {
        void RegistrarNuevaEspecialidad(Especialidad especialidad);
        void ModificarEspecialidad(Especialidad especialidad);
        void RegistrarNuevoEstablecimiento(Establecimiento establecimiento);
        void ModificarEstablecimiento(Establecimiento establecimiento);
        List<Establecimiento> BuscarEstablecimiento(string Nombre);
        List<Especialidad> BuscarEspecialidad(string Nombre);
        Establecimiento ObtenerEstablecimiento(int Id);
        Especialidad ObtenerEspecialidad(int Id);
        List<Establecimiento> ObtenerEstablecimientos();
        List<Especialidad> ObtenerEspecialidades();



    }
}
