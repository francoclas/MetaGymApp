using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioRutina : IRutinaServicio
    {
        private readonly IRepositorioRutina repositorioRutina;
        private readonly IRepositorioEjercicio repositorioEjercicio;

        public ServicioRutina(IRepositorioRutina repositorioRutina,IRepositorioEjercicio repositorio)
        {
            this.repositorioRutina = repositorioRutina;
            this.repositorioEjercicio = repositorio;
        }

        public void AsignarRutina(Rutina rutina, Cliente cliente)
        {
            throw new NotImplementedException();
        }

        public void DesasignarRutina(Rutina rutina, Cliente cliente)
        {
            throw new NotImplementedException();
        }

        public Rutina GenerarNuevaRutina(Rutina rutina)
        {
            throw new NotImplementedException();
        }

        public Ejercicio GenerarNuevoEjercicio(Ejercicio ejercicio)
        {
            repositorioEjercicio.Agregar(ejercicio);
            return ejercicio ;
        }

        public void ModificarEjercicio(Ejercicio ejercicio)
        {
            throw new NotImplementedException();
        }

        public void ModificarRutina(Rutina rutina)
        {
            throw new NotImplementedException();
        }

        public List<Rutina> ObtenerPorProfesional(int profesionalId)
        {
           return repositorioRutina.ObtenerPorProfesional(profesionalId);
        }
    }
}
