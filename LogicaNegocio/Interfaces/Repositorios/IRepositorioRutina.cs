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
        //Gestion
        public List<Rutina> ObtenerPorProfesional(int profesionalId);
        public List<Rutina> BuscarPorNombre(string nombre);
        // Gestión de asignación
        void AsignarRutinaACliente(RutinaAsignada asignacion);
        List<RutinaAsignada> ObtenerAsignacionesPorCliente(int clienteId);
        bool ClienteTieneRutinaAsignada(int clienteId, int rutinaId);
        void RemoverAsignacion(int rutinaAsignadaId);
        List<RutinaAsignada> ObtenerAsignacionesPorRutina(int rutinaId);
        // Gestión de sesiones
        List<SesionRutina> ObtenerSesionesPorCliente(int clienteId);
        SesionRutina? ObtenerSesionPorId(int sesionId);
        List<SesionRutina> ObtenerSesionesPorAsignacion(int rutinaAsignadaId);
        SesionRutina RegistrarSesion(SesionRutina sesion);
        RutinaAsignada? ObtenerAsignacion(int asignacionId);
    }
}
