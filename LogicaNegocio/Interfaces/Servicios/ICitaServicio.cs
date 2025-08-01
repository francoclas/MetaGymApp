using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaApp.DTOS;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface ICitaServicio
    {
        //ABM
        void GenerarNuevaCita(CitaDTO cita);
        void CrearCita(Cita cita);
        //Flujo cita
        void FinalizarCita(CitaDTO cita);
        void ActualizarCita(CitaDTO cita);
        void CancelarCita(CitaDTO cita);
        void NoAsistioCita(CitaDTO cita);
        //Listados

        //Profesional
        List<Cita>BuscarSolicitudesSegunEspecialidades(List<int> especialidadesId);
        List<Cita> SolicitarProximasProfesional(int profesionalID);
        List<Cita> SolicitarHistorialProfesional(int profesionalID);
        //Cliente
        List<Cita> SolicitarProximasCliente(int clienteID);
        List<Cita> SolicitarHistorialCliente(int clienteID);
        //Buscar
        List<Cita> BuscarPorClienteYEstado(int clienteID,EstadoCita estado);
        List<Cita> BuscarPorEstado(EstadoCita estado);
        Cita ObtenerPorId(int citaId);
        public List<DateTime> ObtenerHorariosDisponibles(int profesionalId, DateTime fecha, int duracionMin);
        List<Cita> BuscarSolicitudesSegunTiposAtencion(List<int> tiposAtencionId);
        void ActualizarEntidad(Cita cita);
        void RegistrarCitaPorProfesional(CitaDTO cita);
    }
}
