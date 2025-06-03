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
        void FinalizarCita(Cita cita);
        void ActualizarCita(Cita cita);

        //Listados
        
        //Profesional
        List<Cita> SolicitarProximasProfesional(int profesionalID);
        List<Cita> SolicitarHistorialProfesional(int profesionalID);
        //Cliente
        List<Cita> SolicitarProximasCliente(int clienteID);
        List<Cita> SolicitarHistorialCliente(int clienteID);

        //Buscar
        List<Cita> BuscarPorClienteYEstado(int clienteID,EstadoCita estado);


    }
}
