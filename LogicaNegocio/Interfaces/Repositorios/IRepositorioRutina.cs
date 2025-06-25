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
        //Asignacion
        public void AsignarRutinaACliente(Cliente cliente, Rutina rutina);
        public List<Rutina> ObtenerRutinasAsignadasACliente(int clienteId);
        public bool ClienteTieneRutina(Cliente cliente, Rutina rutina);
        public void RemoverRutinaDeCliente(Cliente cliente, Rutina rutina);
        //Sesiones rutina
        void RegistrarSesion(SesionRutina sesion);
        List<SesionRutina> ObtenerSesionesPorCliente(int clienteId);
        SesionRutina? ObtenerSesionPorId(int sesionId);
    }
}
