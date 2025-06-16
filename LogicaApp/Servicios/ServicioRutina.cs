using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioRutina : IRutinaServicio
    {
        private readonly IRepositorioRutina repositorioRutina;

        public ServicioRutina(IRepositorioRutina repositorioRutina)
        {
            this.repositorioRutina = repositorioRutina;
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
            throw new NotImplementedException();
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
