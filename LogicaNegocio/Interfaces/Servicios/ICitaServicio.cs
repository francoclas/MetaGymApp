using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaApp.DTOS;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS.API;

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
        List<CitaDTO> SolicitarHistorialCliente(int clienteID);
        //Buscar
        List<Cita> BuscarPorClienteYEstado(int clienteID,EstadoCita estado);
        List<Cita> BuscarPorEstado(EstadoCita estado);
        Cita ObtenerPorId(int citaId);
        public List<HorarioDisponibleDTO> ObtenerHorariosDisponibles(int profesionalId);
        List<Cita> BuscarSolicitudesSegunTiposAtencion(List<int> tiposAtencionId);
        void ActualizarEntidad(Cita cita);
        void RegistrarCitaPorProfesional(CitaDTO cita);
        //Api

        List<CitaDTO> ObtenerCitasClientes(int clienteId, int estadoCita);
        CitaDTO ObtenerDetallesCita(int citaId);
        List<CitaDTO> ObtenerTodasCitasClientes(int clienteId);
    }
}
