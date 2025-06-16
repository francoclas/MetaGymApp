using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IRutinaServicio
    {


        //Gestion de Ejercicios
        Ejercicio GenerarNuevoEjercicio(Ejercicio ejercicio);
        void ModificarEjercicio (Ejercicio ejercicio);
        List<Rutina> ObtenerPorProfesional(int profesionalId);
        //Gestion de rutinas
        Rutina GenerarNuevaRutina (Rutina rutina);
            void ModificarRutina (Rutina rutina);
            void AsignarRutina(Rutina rutina, Cliente cliente);
            void DesasignarRutina(Rutina rutina, Cliente cliente);

    }
}
