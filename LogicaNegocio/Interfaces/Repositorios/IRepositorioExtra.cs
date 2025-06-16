using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaDatos.Interfaces.Repos
{
    public interface IRepositorioExtra
    {
        void AltaEspecialidad(Especialidad Nuevo);
        void ModificarEspecialidad(Especialidad EspecialidadMod);
        List<Especialidad> BuscarEspecialidad(string Nombre);
        Especialidad ObtenerEspecialidadId(int Id);
        void AltaEstablecimiento(Establecimiento Nuevo);
        void ModificarEstablecimiento(Establecimiento EstablecimientoMod);
        Establecimiento ObtenerEstablecimientoId(int Id);
        List<Establecimiento> BuscarEstablecimiento(string Nombre);
        List<Establecimiento> ListarEstablecimientos();
        List<Especialidad> ListarEspecialidades();
        void AltaMedia(Media nueva);

    }
}
