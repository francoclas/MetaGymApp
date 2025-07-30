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
            Cita Actualizada = repositorioCita.ObtenerPorId(cita.CitaId);

            Actualizada.Descripcion = cita.Descripcion;

            if (cita.EstablecimientoId != Actualizada.EstablecimientoId)
            {
                Actualizada.EstablecimientoId = cita.EstablecimientoId;
            }

            if (cita.EspecialidadId != Actualizada.EspecialidadId)
            {
                Actualizada.EspecialidadId = cita.EspecialidadId;
            }

            if (cita.TipoAtencionId != Actualizada.TipoAtencionId)
            {
                Actualizada.TipoAtencionId = (int)cita.TipoAtencionId;
            }

            if (cita.FechaAsistencia != Actualizada.FechaAsistencia)
            {
                Actualizada.FechaAsistencia = cita.FechaAsistencia;
            }

            repositorioCita.Actualizar(Actualizada);
        }
        public void ActualizarEntidad(Cita cita)
        {
            if (cita == null)
                throw new CitaException("No se recibió la cita a actualizar.");

            // Validar que exista en el sistema
            Cita original = repositorioCita.ObtenerPorId(cita.Id);
            if (original == null)
                throw new CitaException("La cita no existe en el sistema.");

            // Actualizar campos relevantes
            original.Descripcion = cita.Descripcion;
            original.FechaAsistencia = cita.FechaAsistencia;
            original.EspecialidadId = cita.EspecialidadId;
            original.TipoAtencionId = cita.TipoAtencionId;
            original.EstablecimientoId = cita.EstablecimientoId;
            original.ProfesionalId = cita.ProfesionalId;
            original.Estado = cita.Estado;
            original.Conclusion = cita.Conclusion;

            repositorioCita.Actualizar(original);
        }
        public void GenerarNuevaCita(CitaDTO cita)
        {
            // Obtengo la especialidad, establecimiento y tipo de atención
            Especialidad especialidadAux = repositorioExtra.ObtenerEspecialidadId(cita.EspecialidadId);
            Establecimiento establecimientoAux = repositorioExtra.ObtenerEstablecimientoId(cita.EstablecimientoId);
            TipoAtencion tipoAtencionAux = repositorioExtra.ObtenerTipoAtencionId(cita.TipoAtencionId);

            // Crea la nueva cita con estado EnEspera y sin profesional asignado
            Cita Nueva = new Cita{
                ClienteId = cita.ClienteId,
                Especialidad = especialidadAux,
                Profesional = null,
                Establecimiento = establecimientoAux,
                Descripcion = cita.Descripcion,
                FechaAsistencia = cita.FechaAsistencia,
                TipoAtencion = tipoAtencionAux,
                Estado = EstadoCita.EnEspera
            };

            // Valido la información de la cita
            Nueva.Validar();

            // Cargo al sistema
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

        public List<DateTime> ObtenerHorariosDisponibles(int profesionalId, DateTime fecha, int duracionMin)
        {
            return null;
        }
        public List<Cita> BuscarSolicitudesSegunTiposAtencion(List<int> tiposAtencionId)
        {
            return repositorioCita.BuscarPorEstado(EstadoCita.EnEspera)
                                  .Where(c => tiposAtencionId.Contains(c.TipoAtencionId))
                                  .ToList();
        }
    }

}
