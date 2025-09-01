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
using LogicaNegocio.Interfaces.DTOS.API;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioCita : ICitaServicio
    {
        private readonly IRepositorioCita _repositorioCita;
        private readonly IRepositorioExtra _repositorioExtra;
        private readonly IRepositorioProfesional _repositorioProfesional;
        public ServicioCita(IRepositorioCita repocita, IRepositorioExtra repoex, IRepositorioProfesional repopro)
        {
            _repositorioCita = repocita;
            _repositorioExtra = repoex;
            _repositorioProfesional = repopro;
        }


        //ABM Cita
        public void FinalizarCita(CitaDTO cita)
        {
            //Obtengo Cita 
            Cita Actualizada = _repositorioCita.ObtenerPorId(cita.CitaId);
            Actualizada.FinalizarCita(cita.Conclusion);
            //Mando a sistema
            _repositorioCita.Actualizar(Actualizada);
        }
        public void ActualizarCita(CitaDTO cita)
        {
            Cita actual = _repositorioCita.ObtenerPorId(cita.CitaId);

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

            _repositorioCita.Actualizar(actual);
        }
        public void ActualizarEntidad(Cita cita)
        {
            if (cita == null)
                throw new CitaException("No se recibió la cita a actualizar.");

            // Validar que exista en el sistema
            Cita original = _repositorioCita.ObtenerPorId(cita.Id);
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
            //finalizado o cancelado
            if (cita.Estado == EstadoCita.Finalizada || EstadoCita.Cancelada == cita.Estado)
            {
                original.FechaFinalizacion = cita.FechaFinalizacion;
                original.Conclusion = cita.Conclusion;
            }
            _repositorioCita.Actualizar(original);
        }
        public void GenerarNuevaCita(CitaDTO cita)
        {
            // Obtengo la especialidad, establecimiento y tipo de atención
            Especialidad especialidadAux = _repositorioExtra.ObtenerEspecialidadId(cita.EspecialidadId);
            Establecimiento establecimientoAux = _repositorioExtra.ObtenerEstablecimientoId(cita.EstablecimientoId);
            TipoAtencion tipoAtencionAux = _repositorioExtra.ObtenerTipoAtencionId(cita.TipoAtencionId);

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
            _repositorioCita.Agregar(Nueva);
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

            _repositorioCita.Agregar(cita);
        }

        public void CancelarCita(CitaDTO cita)
        {
            //Obtengo cita
            Cita Cancelada = _repositorioCita.ObtenerPorId(cita.CitaId);
            //Actualizo estado
            Cancelada.CancelarCita(cita.Conclusion);
            //Mando a sistema
            _repositorioCita.Actualizar(Cancelada);

        }

        public void NoAsistioCita(CitaDTO cita)
        {
            //Obtengo cita
            Cita Cancelada = _repositorioCita.ObtenerPorId(cita.CitaId);
            //Actualizo
            Cancelada.ClienteNoAsiste();
            //Mando a sistema
            _repositorioCita.Actualizar(Cancelada);
        }
       
        //Solicitud de listas
        public List<CitaDTO> SolicitarHistorialCliente(int clienteID)
        {
            List<Cita> aux = _repositorioCita.ObtenerPorCliente(clienteID);
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
            List<Cita> Salida = _repositorioCita.ObtenerPorProfesional(profesionalID);
            return Salida;
        }
        public List<Cita> SolicitarProximasCliente(int clienteID)
        {
            List<Cita> Salida = _repositorioCita.ObtenerPorClienteYEstado(clienteID, EstadoCita.Aceptada);
            return Salida;
        }
        public List<Cita> SolicitarProximasProfesional(int profesionalID)
        {
            List<Cita> Salida = _repositorioCita.ObtenerPorProfesionalYEstado(profesionalID, EstadoCita.Aceptada);
            return Salida;
        }
        public List<Cita> BuscarPorClienteYEstado(int clienteID, EstadoCita estado)
        {
            List<Cita> Salida = _repositorioCita.ObtenerPorClienteYEstado(clienteID, estado);
            return Salida;
        }

        public List<Cita> BuscarPorEstado(EstadoCita estado)
        {
            List<Cita> salida = _repositorioCita.BuscarPorEstado(estado);
            return salida;
        }

        public Cita ObtenerPorId(int citaId)
        {
            Cita salida = _repositorioCita.ObtenerPorId(citaId);
            return salida;
        }

        public List<Cita> BuscarSolicitudesSegunEspecialidades(List<int> especialidadesId)
        {
            return _repositorioCita.BuscarPorEstado(EstadoCita.EnEspera)
                                .Where(c => especialidadesId.Contains(c.EspecialidadId))
                                .ToList();
        }

        public List<HorarioDisponibleDTO> ObtenerHorariosDisponibles(int profesionalId)
        {
            List<HorarioDisponibleDTO> salida = new();
            Profesional profesional = _repositorioProfesional.ObtenerPorId(profesionalId);

            DateTime hoy = DateTime.Now.Date;

            //recorro agenda del profesional
            foreach (var agenda in profesional.Agendas.Where(a => a.Activo))
            {
                //por cada dia genero el sector
                int diff = ((int)agenda.Dia - (int)hoy.DayOfWeek + 7) % 7;
                DateTime fechaAgenda = hoy.AddDays(diff);

                DateTime inicioFranja = fechaAgenda.Date + agenda.HoraInicio;
                DateTime finFranja = fechaAgenda.Date + agenda.HoraFin;

                DateTime cursor = inicioFranja;

                while (cursor.AddMinutes(30) <= finFranja)
                {
                    var slot = new HorarioDisponibleDTO
                    {
                        Inicio = cursor,
                        Fin = cursor.AddMinutes(30)
                    };

                    salida.Add(slot);
                    cursor = cursor.AddMinutes(30);
                }
            }

            // 4. Obtener citas aceptadas del profesional
            var citas = profesional.Citas
                .Where(c => c.Estado == EstadoCita.Aceptada && c.FechaAsistencia.HasValue)
                .ToList();

            // 5. Filtrar slots que se solapan con citas
            salida = salida
                .Where(slot => !citas.Any(c =>
                    slot.Inicio < c.FechaAsistencia.Value.AddMinutes(30) &&
                    slot.Fin > c.FechaAsistencia.Value))
                .OrderBy(s => s.Inicio)
                .ToList();

            return salida;
        }
        public List<Cita> BuscarSolicitudesSegunTiposAtencion(List<int> tiposAtencionId)
        {
            return _repositorioCita.BuscarPorEstado(EstadoCita.EnEspera)
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
            List<Cita> aux = _repositorioCita.ObtenerPorCliente(clienteId);
            List<CitaDTO> Salida = new List<CitaDTO>();
            foreach (Cita cita in aux)
            {
                if (cita.Estado == (EstadoCita)estadoCita)
                {
                    CitaDTO ux = new CitaDTO
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
                    };
                    if (cita.Profesional != null)
                    {
                        ux.TelefonoProfesional = cita.Profesional.Telefono;
                        ux.NombreProfesional = cita.Profesional.NombreCompleto;
                    };
                    Salida.Add(ux);
                }

            }
            return Salida;
        }
        public List<CitaDTO> ObtenerTodasCitasClientes(int clienteId)
        {
            List<Cita> aux = _repositorioCita.ObtenerPorCliente(clienteId);
            List<CitaDTO> Salida = new List<CitaDTO>();
            foreach (Cita cita in aux)
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
            return Salida;
        }
        public CitaDTO ObtenerDetallesCita(int citaId)
        {
            Cita au = _repositorioCita.ObtenerPorId(citaId);
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
                FechaAsistencia = (DateTime)au.FechaAsistencia,
                
            };
            if(au.Estado == EstadoCita.Finalizada)
            {
                salida.FechaFinalizacion = (DateTime)au.FechaFinalizacion;
                salida.Conclusion = au.Conclusion;
            }
            if (au.Profesional != null)
            {
                salida.ProfesionalId = au.ProfesionalId;
                salida.NombreProfesional = au.Profesional.NombreCompleto;
                salida.TelefonoProfesional = au.Profesional.Telefono;
            }
            if (au.FechaFinalizacion != null)
                salida.FechaFinalizacion = au.FechaFinalizacion;
            if (au.FechaAsistencia != null)
                au.FechaAsistencia = au.FechaAsistencia;

            return salida;
        }

        
    }
}
