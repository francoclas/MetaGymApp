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
        // Rutinas
        List<Rutina> ObtenerRutinasProfesional(int profesionalId);
        Rutina GenerarNuevaRutina(Rutina rutina);
        void ModificarRutina(Rutina rutina);
        Rutina ObtenerRutinaPorId(int id);
        List<SesionRutina> ObtenerSesionesPorAsignacion(int rutinaAsignadaId);
        // Ejercicios
        Ejercicio GenerarNuevoEjercicio(Ejercicio ejercicio);
        void ModificarEjercicio(Ejercicio ejercicio);
        List<EjercicioDTO> ObtenerTodosEjercicios();
        List<EjercicioDTO> ObtenerEjerciciosProfesional(int profesionalId);
        Ejercicio ObtenerEjercicioId(int id);
        EjercicioDTO ObtenerEjercicioDTOId(int id);

        // Asignaciones
        void AsignarRutinaACliente(int clienteId, int rutinaId);
        List<RutinaAsignada> ObtenerRutinasAsignadasCliente(int clienteId);
        void RemoverAsignacion(int rutinaAsignadaId);
        bool ClienteTieneRutinaAsignada(int clienteId, int rutinaId);
        void ReemplazarAsignaciones(int rutinaId, List<int> nuevosClienteIds);

        // Sesiones
        SesionRutina RegistrarSesion(SesionRutina sesion);
        List<SesionRutina> ObtenerHistorialCliente(int clienteId);
        SesionRutina? ObtenerSesionPorId(int sesionId);
       
        List<Rutina> ObtenerTodasRutinas();
        void AsignarRutina(Rutina rutina, Cliente cliente);
        void DesasignarRutina(Rutina rutina, Cliente cliente);
        List<RutinaAsignada> ObtenerAsignacionesPorRutina(int rutinaId);
        //DTOs
        RutinaAsignadaDTO ObtenerDetalleRutinaAsignadaDTO(int rutinaAsignadaId, int clienteId);

    }
}
