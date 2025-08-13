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
        public ServicioCita(IRepositorioCita repocita, IRepositorioExtra repoex)
        {
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
            Cita actual = repositorioCita.ObtenerPorId(cita.CitaId);

            if (!string.IsNullOrWhiteSpace(cita.Descripcion))
                actual.Descripcion = cita.Descripcion;

            if (cita.EstablecimientoId != actual.EstablecimientoId)
                actual.EstablecimientoId = cita.EstablecimientoId;

            if (cita.EspecialidadId != 0 && cita.EspecialidadId != actual.EspecialidadId)
                actual.EspecialidadId = cita.EspecialidadId;

            if (cita.TipoAtencionId.HasValue && cita.TipoAtencionId != actual.TipoAtencionId)
                actual.TipoAtencionId = (int)cita.TipoAtencionId;

            if (cita.FechaAsistencia != actual.FechaAsistencia)
                actual.FechaAsistencia = cita.FechaAsistencia;

            repositorioCita.Actualizar(actual);
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
            Cita Nueva = new Cita
            {
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
        public void CrearCita(Cita cita)
        {
            if (cita == null)
                throw new ArgumentNullException("La cita no puede ser nula.");

            // Validaciones mínimas
            if (cita.ClienteId == 0 || cita.ProfesionalId == 0 || cita.FechaAsistencia == null)
                throw new Exception("Faltan datos requeridos para registrar la cita.");

            if (cita.FechaAsistencia <= DateTime.Now)
                throw new Exception("La fecha de asistencia debe ser posterior al momento actual.");

            cita.FechaCreacion = DateTime.Now;

            repositorioCita.Agregar(cita);
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
        public List<CitaDTO> SolicitarHistorialCliente(int clienteID)
        {
            List<Cita> aux = repositorioCita.ObtenerPorCliente(clienteID);
            List<CitaDTO> Salida = new List<CitaDTO>();
            foreach (Cita cita in aux)
            {
                Salida.Add(new CitaDTO
                {
                    CitaId = cita.Id,
                    ClienteId = clienteID,
                    Cliente = cita.Cliente,
                    Estado = cita.Estado,
                    EspecialidadId = cita.EspecialidadId,
                    Especialidad = cita.Especialidad,
                    TipoAtencion = cita.TipoAtencion,
                    Establecimiento = cita.Establecimiento,
                    Descripcion = cita.Descripcion,
                    FechaAsistencia = (DateTime)cita.FechaAsistencia,
                    FechaCreacion = cita.FechaCreacion,
                    FechaFinalizacion = cita.FechaFinalizacion,
                    Conclusion = cita.Conclusion
                });
            }
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

        public void RegistrarCitaPorProfesional(CitaDTO cita)
        {
            //mapeo
            Cita nueva = new Cita
            {
                ProfesionalId = cita.ProfesionalId,
                ClienteId = cita.ClienteId,
                EspecialidadId = cita.EspecialidadId,
                TipoAtencionId = (int)cita.TipoAtencionId,
                EstablecimientoId = cita.EstablecimientoId,
                Descripcion = cita.Descripcion,
                FechaAsistencia = cita.FechaAsistencia,
                Estado = EstadoCita.Aceptada
            };
            CrearCita(nueva);
        }

        public List<CitaDTO> ObtenerCitasClientes(int clienteId, int estadoCita)
        {
            List<Cita> aux = repositorioCita.ObtenerPorCliente(clienteId);
            List<CitaDTO> Salida = new List<CitaDTO>();
            foreach (Cita cita in aux)
            {
                if (cita.Estado == (EstadoCita)estadoCita)
                {
                    Salida.Add(new CitaDTO
                    {
                        CitaId = cita.Id,
                        ClienteId = clienteId,
                        Cliente = cita.Cliente,
                        Estado = cita.Estado,
                        EspecialidadId = cita.EspecialidadId,
                        Especialidad = cita.Especialidad,
                        TipoAtencion = cita.TipoAtencion,
                        Establecimiento = cita.Establecimiento,
                        Descripcion = cita.Descripcion,
                        FechaAsistencia = (DateTime)cita.FechaAsistencia,
                        FechaCreacion = cita.FechaCreacion,
                        FechaFinalizacion = cita.FechaFinalizacion,
                        Conclusion = cita.Conclusion,
                        NombreProfesional = cita.Profesional.NombreCompleto,
                        TelefonoProfesional = cita.Profesional.Telefono
                        

                        
                    });
                }

            }
            return Salida;
        }

        public CitaDTO ObtenerDetallesCita(int citaId)
        {
            Cita au = repositorioCita.ObtenerPorId(citaId);
            if (au == null) { throw new Exception("La cita no existe o se elimino."); }
            CitaDTO salida = new CitaDTO
            {
                CitaId = citaId,
                Cliente = au.Cliente,
                Estado = au.Estado,
                Especialidad = au.Especialidad,
                TipoAtencion = au.TipoAtencion,
                Establecimiento = au.Establecimiento,
                Descripcion = au.Descripcion,
                FechaCreacion = (DateTime)au.FechaCreacion,
                ProfesionalId = au.ProfesionalId,
                NombreProfesional = au.Profesional.NombreCompleto,
                TelefonoProfesional = au.Profesional.Telefono
            };

            if (au.FechaFinalizacion != null)
                salida.FechaFinalizacion = au.FechaFinalizacion;
            if (au.FechaAsistencia != null)
                au.FechaAsistencia = au.FechaAsistencia;

            return salida;
        }

    }
}
