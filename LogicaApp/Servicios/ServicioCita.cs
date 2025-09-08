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

        // =========================
        // ABM de Cita
        // =========================

        // Marca una cita como FINALIZADA con su conclusión.
        public void FinalizarCita(CitaDTO cita)
        {
            // Busco la cita real
            Cita Actualizada = _repositorioCita.ObtenerPorId(cita.CitaId);
            // Le digo a la entidad que se finalice (regla de dominio adentro)
            Actualizada.FinalizarCita(cita.Conclusion);
            // Persisto
            _repositorioCita.Actualizar(Actualizada);
        }

        // Update puntuales sobre una cita
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

        // Update completo de entidad.
        public void ActualizarEntidad(Cita cita)
        {
            if (cita == null)
                throw new CitaException("No se recibió la cita a actualizar.");

            // Valido que exista
            Cita original = _repositorioCita.ObtenerPorId(cita.Id);
            if (original == null)
                throw new CitaException("La cita no existe en el sistema.");

            // Copio campos
            original.Descripcion = cita.Descripcion;
            original.FechaAsistencia = cita.FechaAsistencia;
            original.EspecialidadId = cita.EspecialidadId;
            original.TipoAtencionId = cita.TipoAtencionId;
            original.EstablecimientoId = cita.EstablecimientoId;
            original.ProfesionalId = cita.ProfesionalId;
            original.Estado = cita.Estado;

            // Si terminó o se canceló, guardo fecha de cierre y conclusión
            if (cita.Estado == EstadoCita.Finalizada || EstadoCita.Cancelada == cita.Estado)
            {
                original.FechaFinalizacion = cita.FechaFinalizacion;
                original.Conclusion = cita.Conclusion;
            }
            _repositorioCita.Actualizar(original);
        }

        // Crea una nueva cita solicitada por CLIENTE (queda EnEspera y sin profesional).
        public void GenerarNuevaCita(CitaDTO cita)
        {
            // Traigo referencias necesarias
            Especialidad especialidadAux = _repositorioExtra.ObtenerEspecialidadId(cita.EspecialidadId);
            Establecimiento establecimientoAux = _repositorioExtra.ObtenerEstablecimientoId(cita.EstablecimientoId);
            TipoAtencion tipoAtencionAux = _repositorioExtra.ObtenerTipoAtencionId(cita.TipoAtencionId);

            // Armo la cita
            Cita Nueva = new Cita
            {
                ClienteId = cita.ClienteId,
                Especialidad = especialidadAux,
                Profesional = null, // sin asignar
                Establecimiento = establecimientoAux,
                Descripcion = cita.Descripcion,
                FechaCreacion = DateTime.UtcNow,
                FechaAsistencia = cita.FechaAsistencia,
                TipoAtencion = tipoAtencionAux,
                Estado = EstadoCita.EnEspera
            };
            //valido
            Nueva.Validar();

            // Guardo
            _repositorioCita.Agregar(Nueva);
        }

        // Crea cita directa.
        public void CrearCita(Cita cita)
        {
            if (cita == null)
                throw new ArgumentNullException("La cita no puede ser nula.");

            // Valido datos base
            if (cita.ClienteId == 0 || cita.ProfesionalId == 0 || cita.FechaAsistencia == null)
                throw new Exception("Faltan datos requeridos para registrar la cita.");

            if (cita.FechaAsistencia <= DateTime.Now)
                throw new Exception("La fecha de asistencia debe ser posterior al momento actual.");

            cita.FechaCreacion = DateTime.Now;

            _repositorioCita.Agregar(cita);
        }

        // Pone la cita en estado CANCELADA con una razón.
        public void CancelarCita(CitaDTO cita)
        {
            Cita Cancelada = _repositorioCita.ObtenerPorId(cita.CitaId);
            Cancelada.CancelarCita(cita.Conclusion);
            _repositorioCita.Actualizar(Cancelada);
        }

        // Marca que el cliente NO asistió.
        public void NoAsistioCita(CitaDTO cita)
        {
            Cita Cancelada = _repositorioCita.ObtenerPorId(cita.CitaId);
            Cancelada.ClienteNoAsiste();
            _repositorioCita.Actualizar(Cancelada);
        }

        // =========================
        // Consultas y listados
        // =========================

        // Historial completo del cliente (mapeado a DTO liviano para la vista).
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

        // Historial del profesional tal cual (entidades).
        public List<Cita> SolicitarHistorialProfesional(int profesionalID)
        {
            List<Cita> Salida = _repositorioCita.ObtenerPorProfesional(profesionalID);
            return Salida;
        }

        // Próximas del cliente (estado Aceptada).
        public List<Cita> SolicitarProximasCliente(int clienteID)
        {
            List<Cita> Salida = _repositorioCita.ObtenerPorClienteYEstado(clienteID, EstadoCita.Aceptada);
            return Salida;
        }

        // Próximas del profesional (estado Aceptada).
        public List<Cita> SolicitarProximasProfesional(int profesionalID)
        {
            List<Cita> Salida = _repositorioCita.ObtenerPorProfesionalYEstado(profesionalID, EstadoCita.Aceptada);
            return Salida;
        }

        // Filtro de historial cliente por estado puntual.
        public List<Cita> BuscarPorClienteYEstado(int clienteID, EstadoCita estado)
        {
            List<Cita> Salida = _repositorioCita.ObtenerPorClienteYEstado(clienteID, estado);
            return Salida;
        }

        // Citas por estado (cualquiera).
        public List<Cita> BuscarPorEstado(EstadoCita estado)
        {
            List<Cita> salida = _repositorioCita.BuscarPorEstado(estado);
            return salida;
        }

        // Leer una cita por Id.
        public Cita ObtenerPorId(int citaId)
        {
            Cita salida = _repositorioCita.ObtenerPorId(citaId);
            return salida;
        }

        // Solicitudes en espera que matchean por ESPECIALIDADES del profesional.
        public List<Cita> BuscarSolicitudesSegunEspecialidades(List<int> especialidadesId)
        {
            return _repositorioCita.BuscarPorEstado(EstadoCita.EnEspera)
                                .Where(c => especialidadesId.Contains(c.EspecialidadId))
                                .ToList();
        }

        // Calcula slots de 30 minutos para la PRÓXIMA ocurrencia de cada agenda activa del profesional.
        // Luego descarta los que se pisan con citas aceptadas.
        public List<HorarioDisponibleDTO> ObtenerHorariosDisponibles(int profesionalId)
        {
            List<HorarioDisponibleDTO> salida = new();
            Profesional profesional = _repositorioProfesional.ObtenerPorId(profesionalId);

            DateTime hoy = DateTime.Now.Date;

            // 1) Recorro agendas activas
            foreach (var agenda in profesional.Agendas.Where(a => a.Activo))
            {
                // 2) Busco la próxima fecha de ese día de la semana
                int diff = ((int)agenda.Dia - (int)hoy.DayOfWeek + 7) % 7;
                DateTime fechaAgenda = hoy.AddDays(diff);

                // 3) Construyo franja [inicio, fin)
                DateTime inicioFranja = fechaAgenda.Date + agenda.HoraInicio;
                DateTime finFranja = fechaAgenda.Date + agenda.HoraFin;

                // 4) Genero slots de 30 minutos
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

            // 5) Tomo citas aceptadas del pro (para filtrar solapes)
            var citas = profesional.Citas
                .Where(c => c.Estado == EstadoCita.Aceptada && c.FechaAsistencia.HasValue)
                .ToList();

            // 6) Me quedo solo con slots libres (sin choque de horarios)
            salida = salida
                .Where(slot => !citas.Any(c =>
                    slot.Inicio < c.FechaAsistencia.Value.AddMinutes(30) &&
                    slot.Fin > c.FechaAsistencia.Value))
                .OrderBy(s => s.Inicio)
                .ToList();

            return salida;
        }

        // Solicitudes en espera que matchean por TIPOS DE ATENCIÓN del profesional.
        public List<Cita> BuscarSolicitudesSegunTiposAtencion(List<int> tiposAtencionId)
        {
            return _repositorioCita.BuscarPorEstado(EstadoCita.EnEspera)
                                  .Where(c => tiposAtencionId.Contains(c.TipoAtencionId))
                                  .ToList();
        }

        // Registrar cita creada por el profesional (se acepta directo).
        public void RegistrarCitaPorProfesional(CitaDTO cita)
        {
            // mapeo DTO -> entidad
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

        // Citas del cliente filtradas por estado (mapeo a DTO para la UI).
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
                    }
                    ;
                    Salida.Add(ux);
                }
            }
            return Salida;
        }

        // Todas las citas del cliente (sin filtrar) -> DTO para UI.
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

        // Detalle de una cita puntual para mostrar en pantalla.
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

            // Datos extra si aplica
            if (au.Estado == EstadoCita.Finalizada)
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