using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
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


        //ABM Cita
        public void FinalizarCita(CitaDTO cita)
        {
            //Obtengo Cita 
            Cita Actualizada = repositorioCita.ObtenerPorId(cita.CitaId);
            Actualizada.FinalizarCita(cita.Conclusion);
            //Mando a sistema
            repositorioCita.Actualizar(Actualizada);
        }
        public void ActualizarCita(CitaDTO cita)
        {
            //Valido que exista la cita en el sistema
            Cita Actualizada = repositorioCita.ObtenerPorId(cita.CitaId);
            //Actualizo
            Actualizada.Descripcion = cita.Descripcion;
            if (cita.EstablecimientoId != Actualizada.EstablecimientoId)
            {
                Actualizada.EstablecimientoId = cita.EstablecimientoId;
            }
            if (cita.EspecialidadId != Actualizada.EspecialidadId)
            {
                Actualizada.EspecialidadId = cita.EspecialidadId;
            }
            if (cita.FechaAsistencia != Actualizada.FechaAsistencia)
            {
                Actualizada.FechaAsistencia = cita.FechaAsistencia;
            }

                //Mando a sistema
                repositorioCita.Actualizar(Actualizada);
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

        public void CancelarCita(CitaDTO cita)
        {
            //Obtengo cita
            Cita Cancelada = repositorioCita.ObtenerPorId(cita.CitaId);
            //Actualizo estado
            Cancelada.CancelarCita(cita.Conclusion);
            //Mando a sistema
            repositorioCita.Actualizar(Cancelada);

        }

        public void NoAsistioCita(CitaDTO cita)
        {
            //Obtengo cita
            Cita Cancelada = repositorioCita.ObtenerPorId(cita.CitaId);
            //Actualizo
            Cancelada.ClienteNoAsiste();
            //Mando a sistema
            repositorioCita.Actualizar(Cancelada);
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
        //Solicitud de listas
        public List<Cita> SolicitarHistorialCliente(int clienteID)
        {
            List<Cita> Salida = repositorioCita.ObtenerPorCliente(clienteID);
            return Salida;
        }
        public List<Cita> SolicitarHistorialProfesional(int profesionalID)
        {  
            List<Cita> Salida = repositorioCita.ObtenerPorProfesional(profesionalID);
            return Salida;
        }
        public List<Cita> SolicitarProximasCliente(int clienteID)
        {
            List<Cita> Salida = repositorioCita.ObtenerPorClienteYEstado(clienteID, EstadoCita.Aceptada);
            return Salida;
        }
        public List<Cita> SolicitarProximasProfesional(int profesionalID)
        {
            List<Cita> Salida = repositorioCita.ObtenerPorProfesionalYEstado(profesionalID, EstadoCita.Aceptada);
            return Salida;
        }
        public List<Cita> BuscarPorClienteYEstado(int clienteID, EstadoCita estado)
        {
            List<Cita> Salida = repositorioCita.ObtenerPorClienteYEstado(clienteID, estado);
            return Salida;
        }

        public List<Cita> BuscarPorEstado(EstadoCita estado)
        {
            List<Cita> salida = repositorioCita.BuscarPorEstado(estado);
            return salida;
        }

        public Cita ObtenerPorId(int citaId)
        {
            Cita salida = repositorioCita.ObtenerPorId(citaId);
            return salida;
        }

        public List<Cita> BuscarSolicitudesSegunEspecialidades(List<int> especialidadesId)
        {
            return repositorioCita.BuscarPorEstado(EstadoCita.EnEspera)
                                .Where(c => especialidadesId.Contains(c.EspecialidadId))
                                .ToList();
        }
    }

}
