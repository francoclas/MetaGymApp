using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaApp.DTOS;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Excepciones;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioCita : ICitaServicio
    {
        private readonly IRepositorioCita repositorioCita;
        private readonly IRepositorioExtra repositorioExtra;
        public ServicioCita(IRepositorioCita repocita,IRepositorioExtra repoex) { 
            repositorioCita = repocita;
            repositorioExtra = repoex;
        }
        public void ActualizarCita(Cita cita)
        {
            //Valido que exista la cita en el sistema
            ValidarExisteCita(cita);
            //Actualizo
            repositorioCita.Actualizar(cita);
        }

        public List<Cita> BuscarPorClienteYEstado(int clienteID, EstadoCita estado)
        {
            return repositorioCita.ObtenerPorClienteYEstado(clienteID, estado);
        }

        public void FinalizarCita(Cita cita)
        {
            throw new NotImplementedException();
        } 

        public void GenerarNuevaCita(CitaDTO cita)
        {
            //Obtengo la especialidad y el establecimiento
            Especialidad especialidadAux = repositorioExtra.ObtenerEspecialidadId(cita.EspecialidadId);
            Establecimiento establecimientoAux = repositorioExtra.ObtenerEstablecimientoId(cita.EstablecimientoId);
            Cita Nueva = new Cita(cita.ClienteId,especialidadAux, establecimientoAux,cita.Descripcion,cita.FechaAsistencia);
            //valido informacion de la cita
            Nueva.Validar();
            //Cargo al sistema
            repositorioCita.Agregar(Nueva);
        }

        public List<Cita> SolicitarHistorialCliente(int clienteID)
        {
            throw new NotImplementedException();
        }

        public List<Cita> SolicitarHistorialProfesional(int profesionalID)
        {  

            List<Cita> Salida = repositorioCita.ObtenerPorProfesional(profesionalID);
            return Salida;
        }
        public List<Cita> SolicitarProximasCliente(int clienteID)
        {

            throw new NotImplementedException();
        }

        public List<Cita> SolicitarProximasProfesional(int profesionalID)
        {
            throw new NotImplementedException();
        }

       private bool ValidarExisteCita(Cita cita)
        {
            if (cita == null)
            {
                throw new CitaException("No se recibio cita a consultar.");
            }
            if (!repositorioCita.ExisteCita(cita))
            {
                throw new CitaException("No se encontro cita en el sistema.");
            }
            return true;
        }
    }
    
}
