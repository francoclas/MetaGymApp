using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioExtras : IExtraServicio
    {
        private readonly IRepositorioExtra repoExtras;
        public ServicioExtras(IRepositorioExtra repo)
        {
            repoExtras = repo;
        }

        public List<Especialidad> BuscarEspecialidad(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                throw new Exception("La especialidad a buscar no puede estar vacia.");
            }
            return repoExtras.BuscarEspecialidad(Nombre);
        }

        public List<Establecimiento> BuscarEstablecimiento(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                throw new Exception("El establecimiento abuscar no puede estar vacio.");
            }
            return repoExtras.BuscarEstablecimiento(Nombre);
        }

        public void ModificarEspecialidad(Especialidad especialidad)
        {
            throw new NotImplementedException();
        }

        public void ModificarEstablecimiento(Establecimiento establecimiento)
        {
            throw new NotImplementedException();
        }

        public Especialidad ObtenerEspecialidad(int Id)
        {
            return repoExtras.ObtenerEspecialidadId(Id);
        }

        public List<Especialidad> ObtenerEspecialidades()
        {
            return repoExtras.ListarEspecialidades();
        }

        public Establecimiento ObtenerEstablecimiento(int Id)
        {
            return repoExtras.ObtenerEstablecimientoId(Id);
        }

        public List<Establecimiento> ObtenerEstablecimientos()
        {
            return repoExtras.ListarEstablecimientos();
        }

        public void RegistrarNuevaEspecialidad(Especialidad especialidad)
        {
            //Valido datos
            if (string.IsNullOrEmpty(especialidad.NombreEspecialidad))
            {
                throw new Exception("El nombre de la especialidad no puede ser vacio.");
            }
            if (string.IsNullOrEmpty(especialidad.DescripcionEspecialidad))
            {
                throw new Exception("Debe ingresar una descripcion de la especialidad.");
            }
            //Envio al repositorio
            repoExtras.AltaEspecialidad(especialidad);
        }

        public void RegistrarNuevoEstablecimiento(Establecimiento establecimiento)
        {
            //Valido datos
            if (string.IsNullOrEmpty(establecimiento.Nombre))
            {
                throw new Exception("El nombre del establecimiento no puede ser vacio.");
            }
            if (string.IsNullOrEmpty(establecimiento.Direccion))
            {
                throw new Exception("Debe ingresar una direccion del establecimiento.");
            }
            //Envio al repositorio
            repoExtras.AltaEstablecimiento(establecimiento);
        }
    }
}
