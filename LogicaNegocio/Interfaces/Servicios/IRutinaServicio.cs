using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IRutinaServicio
    {


        //Gestion de Ejercicios
        Ejercicio GenerarNuevoEjercicio(Ejercicio ejercicio);
        void ModificarEjercicio (Ejercicio ejercicio);
        List<EjercicioDTO> ObtenerTodosEjercicios();
        List<EjercicioDTO> ObtenerEjerciciosProfesional(int Id);
        //Gestion de rutinas
        List<Rutina> ObtenerRutinasProfesional(int profesionalId);
        List<Rutina> ObtenerTodasRutinas();
        Rutina GenerarNuevaRutina (Rutina rutina);
            void ModificarRutina (Rutina rutina);
            void AsignarRutina(Rutina rutina, Cliente cliente);
            void DesasignarRutina(Rutina rutina, Cliente cliente);
        EjercicioDTO ObtenerEjercicioDTOId(int id);
        Ejercicio ObtenerEjercicioId(int id);
    }
}
