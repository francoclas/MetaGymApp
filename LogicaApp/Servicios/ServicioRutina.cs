using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaApp.Excepciones;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.EntityFrameworkCore;

namespace LogicaApp.Servicios
{
    public class ServicioRutina : IRutinaServicio
    {
        // Repositorios de acceso a datos
        private readonly IRepositorioRutina _repositorioRutina;
        private readonly IRepositorioEjercicio _repositorioEjercicio;

        // Servicio de notificaciones para avisos al cliente
        private readonly INotificacionServicio _notificacionServicio;

        // Inyección de dependencias
        public ServicioRutina(IRepositorioRutina repositorioRutina, IRepositorioEjercicio repositorio, INotificacionServicio inotificacionServicio)
        {
            this._repositorioRutina = repositorioRutina;
            this._repositorioEjercicio = repositorio;
            this._notificacionServicio = inotificacionServicio;
        }

        // =======================
        // Asignaciones de rutinas
        // =======================

        public void AsignarRutina(Rutina rutina, Cliente cliente)
        {
            // Pendiente de implementación (no usado en web actual)
            throw new NotImplementedException();
        }

        // Asigna una rutina a un cliente si aún no la tiene asignada
        public void AsignarRutinaACliente(int clienteId, int rutinaId)
        {
            if (!ClienteTieneRutinaAsignada(clienteId, rutinaId))
            {
                var nuevaAsignacion = new RutinaAsignada
                {
                    ClienteId = clienteId,
                    RutinaId = rutinaId,
                    FechaAsignacion = DateTime.Now
                };
                _repositorioRutina.AsignarRutinaACliente(nuevaAsignacion);

                // Notifica al cliente sobre la nueva rutina
                _notificacionServicio.NotificarRutinaAsignada(clienteId, rutinaId);
            }
        }

        // Reemplaza todas las asignaciones de una rutina por un nuevo conjunto de clientes
        public void ReemplazarAsignaciones(int rutinaId, List<int> nuevosClienteIds)
        {
            var existentes = _repositorioRutina.ObtenerAsignacionesPorRutina(rutinaId);

            // Remover todos los actuales
            foreach (var a in existentes)
            {
                _repositorioRutina.RemoverAsignacion(a.Id);
            }

            // Asignar los nuevos (con notificación)
            foreach (var id in nuevosClienteIds)
            {
                _repositorioRutina.AsignarRutinaACliente(new RutinaAsignada
                {
                    ClienteId = id,
                    RutinaId = rutinaId,
                    FechaAsignacion = DateTime.Now
                });
                _notificacionServicio.NotificarRutinaAsignada(id, rutinaId);
            }
        }

        // =======================
        // Eliminaciones
        // =======================

        // Elimina una rutina si existe
        public bool EliminarRutina(int rutinaId)
        {
            Rutina existe = _repositorioRutina.ObtenerPorId(rutinaId);
            if (existe == null) return false;

            _repositorioRutina.Eliminar(rutinaId);
            return true;
        }

        // Consulta si un cliente ya tiene asignada cierta rutina
        public bool ClienteTieneRutinaAsignada(int clienteId, int rutinaId)
        {
            return _repositorioRutina.ClienteTieneRutinaAsignada(clienteId, rutinaId);
        }

        // Elimina un ejercicio si existe; captura excepción de integridad (sesiones/mediciones)
        public bool EliminarEjercicio(int ejercicioId)
        {
            Ejercicio existe = _repositorioEjercicio.ObtenerPorId(ejercicioId);
            if (existe == null) return false;
            try
            {
                _repositorioEjercicio.Eliminar(ejercicioId);
            }
            catch (Exception e)
            {
                throw new Exception("No se puede eliminar ejercicio porque existen mediciones con sesiones de entrenamiento registradas.");
            }
            return true;
        }

        //Sin implementar se realiza desde la misma edicio nde rutina
        public void DesasignarRutina(Rutina rutina, Cliente cliente)
        {
            throw new NotImplementedException();
        }

        // =======================
        // Altas / Modificaciones
        // =======================

        public Rutina GenerarNuevaRutina(Rutina rutina)
        {
            _repositorioRutina.Agregar(rutina);
            return rutina;
        }

        public Ejercicio GenerarNuevoEjercicio(Ejercicio ejercicio)
        {
            _repositorioEjercicio.Agregar(ejercicio);
            return ejercicio;
        }

        public void ModificarEjercicio(Ejercicio ejercicio)
        {
            try
            {
                _repositorioEjercicio.Actualizar(ejercicio);
            }
            catch (Exception e)
            {
                throw new Exception("No se puede eliminar medicion, porque ya existen sesiones registradas.");
            }
        }

        public void ModificarRutina(Rutina rutina)
        {
            _repositorioRutina.Actualizar(rutina);
        }

        // =======================
        // Consultas / DTOs
        // =======================

        // Obtiene un ejercicio y lo mapea a DTO
        public EjercicioDTO ObtenerEjercicioDTOId(int id)
        {
            Ejercicio ejercicio = _repositorioEjercicio.ObtenerPorId(id);
            if (ejercicio == null) return null;

            EjercicioDTO e = new EjercicioDTO
            {
                Id = ejercicio.Id,
                ProfesionalId = ejercicio.ProfesionalId,
                Nombre = ejercicio.Nombre,
                Tipo = ejercicio.Tipo,
                GrupoMuscular = ejercicio.GrupoMuscular,
                Mediciones = ejercicio.Mediciones,
                Instrucciones = ejercicio.Instrucciones
            };
            if (ejercicio.Medias.Any())
            {
                e.Media = ejercicio.Medias[0];
                e.Medias = ejercicio.Medias;
            }
            else
            {
                e.Medias = new List<Media>();
                e.Medias.Add(new Media { Url = "/MediaWeb/Default/ejercicio_default.jpg"});
                e.Media = new Media { Url = "/MediaWeb/Default/ejercicio_default.jpg" };
            }
                return e;
        }

        public Ejercicio ObtenerEjercicioId(int id)
        {
            return _repositorioEjercicio.ObtenerPorId(id);
        }

        public List<EjercicioDTO> ObtenerEjerciciosProfesional(int Id)
        {
            return MapeoEjercicioDTO(_repositorioEjercicio.ObtenerPorProfesional(Id));
        }

        public List<RutinaAsignada> ObtenerRutinasAsignadasCliente(int clienteId)
        {
            return _repositorioRutina.ObtenerAsignacionesPorCliente(clienteId);
        }

        public List<Rutina> ObtenerRutinasProfesional(int profesionalId)
        {
            return _repositorioRutina.ObtenerPorProfesional(profesionalId);
        }

        public SesionRutina? ObtenerSesionPorId(int sesionId)
        {
            return _repositorioRutina.ObtenerSesionPorId(sesionId);
        }

        public List<Rutina> ObtenerTodasRutinas()
        {
            return _repositorioRutina.ObtenerTodos().ToList();
        }

        public List<EjercicioDTO> ObtenerTodosEjercicios()
        {
            return MapeoEjercicioDTO(_repositorioEjercicio.ObtenerTodos().ToList());
        }

        // =======================
        // Registro de sesiones
        // =======================

        public SesionRutina RegistrarSesion(SesionRutina sesion)
        {
            // Validaciones básicas de entrada
            if (sesion == null)
                throw new Exception("La sesión no puede ser nula");

            var rutinaAsignada = _repositorioRutina.ObtenerAsignacion((int)sesion.RutinaAsignadaId);
            if (rutinaAsignada == null)
                throw new Exception("No se encontró la rutina asignada");

            if (rutinaAsignada.ClienteId != sesion.ClienteId)
                throw new Exception("No coincide el cliente con la rutina asignada");

            if (sesion.EjerciciosRealizados == null || !sesion.EjerciciosRealizados.Any())
                throw new Exception("Debe registrar al menos un ejercicio");

            // Snapshot de datos de la rutina (nombre y tipo) para historial
            sesion.NombreRutinaHistorial = rutinaAsignada.Rutina.NombreRutina;
            sesion.TipoRutinaHistorial = rutinaAsignada.Rutina.Tipo;

            // Itera ejercicios realizados y valida/inyecta snapshot de cada ejercicio
            foreach (var ej in sesion.EjerciciosRealizados)
            {
                var ejercicioOriginal = _repositorioEjercicio.ObtenerPorId((int)ej.EjercicioId);
                if (ejercicioOriginal == null)
                    throw new Exception($"No se encontró el ejercicio con ID {ej.EjercicioId}");

                // Copia de campos históricos del ejercicio
                ej.NombreHistorial = ejercicioOriginal.Nombre;
                ej.TipoHistorial = ejercicioOriginal.Tipo;
                ej.GrupoMuscularHistorial = ejercicioOriginal.GrupoMuscular;
                ej.InstruccionesHistorial = ejercicioOriginal.Instrucciones;
                ej.ImagenUrlHistorial = ejercicioOriginal.Medias.FirstOrDefault()?.Url ?? "";

                if (ej.SeRealizo)
                {
                    // Si marcó como realizado debe tener series
                    if (ej.Series == null || !ej.Series.Any())
                        throw new Exception($"El ejercicio {ejercicioOriginal.Nombre} está marcado como realizado pero no tiene series");

                    // Validación de mediciones obligatorias del ejercicio
                    var obligatorias = ejercicioOriginal.Mediciones.Where(m => m.EsObligatoria).ToList();
                    foreach (var ob in obligatorias)
                    {
                        if (!ej.ValoresMediciones.Any(vm => vm.MedicionId == ob.Id && !string.IsNullOrWhiteSpace(vm.Valor)))
                            throw new Exception($"Falta valor para medición obligatoria '{ob.Nombre}' del ejercicio {ejercicioOriginal.Nombre}");
                    }
                }
            }

            // Marca fecha de realización y delega persistencia
            sesion.FechaRealizada = DateTime.Now;
            return _repositorioRutina.RegistrarSesion(sesion);
        }

        // Reemplaza la lista de ejercicios de una rutina 
        public void ActualizarEjerciciosRutina(Rutina rutina, List<int> nuevosIds)
        {
            _repositorioRutina.ActualizarRutina(rutina, nuevosIds);
        }

        public List<SesionRutina> ObtenerSesionesPorAsignacion(int rutinaAsignadaId)
        {
            return _repositorioRutina.ObtenerSesionesPorAsignacion(rutinaAsignadaId);
        }

        public void RemoverAsignacion(int rutinaAsignadaId)
        {
            _repositorioRutina.RemoverAsignacion(rutinaAsignadaId);
        }

        // Funciones de mapeo

        private List<EjercicioDTO> MapeoEjercicioDTO(List<Ejercicio> Lista)
        {
            List<EjercicioDTO> salida = new List<EjercicioDTO>();
            foreach (var item in Lista)
            {
                salida.Add(new EjercicioDTO
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Tipo = item.Tipo,
                    GrupoMuscular = item.GrupoMuscular,
                    Medias = item.Medias,
                    Media = item.Medias.FirstOrDefault(m => m.Tipo == Enum_TipoMedia.Imagen),
                    ProfesionalId = item.ProfesionalId
                });
            }
            return salida;
        }

        // =======================
        // Más consultas
        // =======================

        public Rutina ObtenerRutinaPorId(int id)
        {
            return _repositorioRutina.ObtenerPorId(id);
        }

        public List<RutinaAsignada> ObtenerAsignacionesPorRutina(int rutinaId)
        {
            return _repositorioRutina.ObtenerAsignacionesPorRutina(rutinaId);
        }

        // Detalle completo de una asignación para vistas (rutina + sesiones + ejercicios ordenados)
        public RutinaAsignadaDTO ObtenerDetalleRutinaAsignadaDTO(int rutinaAsignadaId, int clienteId)
        {
            var asignacion = _repositorioRutina.ObtenerAsignacionesPorCliente(clienteId)
                .FirstOrDefault(a => a.Id == rutinaAsignadaId && a.ClienteId == clienteId);

            if (asignacion == null) return null;

            var rutina = _repositorioRutina.ObtenerPorId(asignacion.RutinaId);
            var sesiones = _repositorioRutina.ObtenerSesionesPorAsignacion(rutinaAsignadaId);

            var dto = new RutinaAsignadaDTO
            {
                RutinaAsignadaId = asignacion.Id,
                NombreRutina = rutina.NombreRutina,
                Tipo = rutina.Tipo,
                FechaAsignacion = asignacion.FechaAsignacion,
                Sesiones = sesiones,
                Ejercicios = rutina.Ejercicios
                    .OrderBy(e => e.Orden)
                    .Select(e => new EjercicioDTO
                    {
                        Id = e.Ejercicio.Id,
                        Nombre = e.Ejercicio.Nombre,
                        Tipo = e.Ejercicio.Tipo,
                        GrupoMuscular = e.Ejercicio.GrupoMuscular,
                        Instrucciones = e.Ejercicio.Instrucciones,
                        ProfesionalId = e.Ejercicio.ProfesionalId,
                        Medias = e.Ejercicio.Medias,
                        Media = e.Ejercicio.Medias?.FirstOrDefault(m => m.Tipo == Enum_TipoMedia.Imagen)
                    }).ToList()
            };

            return dto;
        }

        // Historial (sesiones) resumido para cliente
        public List<SesionEntrenadaDTO> ObtenerHistorialClienteDTO(int clienteId)
        {
            List<SesionEntrenadaDTO> salida = new List<SesionEntrenadaDTO>();
            foreach (var sesion in _repositorioRutina.ObtenerSesionesPorCliente(clienteId))
            {
                SesionEntrenadaDTO aux = new SesionEntrenadaDTO
                {
                    SesionRutinaId = sesion.Id,
                    NombreCliente = sesion.Cliente.NombreCompleto,
                    FechaRealizada = sesion.FechaRealizada,
                    DuracionMin = sesion.DuracionMin,
                };

                if (sesion.RutinaAsignada != null)
                    aux.NombreRutina = sesion.RutinaAsignada.Rutina.NombreRutina;
                if (!string.IsNullOrEmpty(sesion.NombreRutinaHistorial))
                    aux.NombreRutina = sesion.NombreRutinaHistorial;
                if (!salida.Contains(aux))
                {
                    salida.Add(aux);
                }
            }

            return salida;
        }

        // Devuelve el nombre de la rutina por Id 
        public string ObtenerNombreRutina(int idRutina)
        {
            Rutina salida = _repositorioRutina.ObtenerPorId(idRutina);
            return salida.NombreRutina;
        }

        // Sesiones completas del cliente
        public List<SesionRutina> ObtenerSesionesCliente(int clienteId)
        {
            return _repositorioRutina.ObtenerSesionesPorCliente(clienteId);
        }

        // Detalle completo de una sesión entrenada (incluye series y mediciones)
        public SesionEntrenadaDTO ObtenerSesionEntrenamiento(int sesionId)
        {
            SesionRutina sesion = _repositorioRutina.ObtenerSesionPorId(sesionId);

            var dto = new SesionEntrenadaDTO
            {
                SesionRutinaId = sesion.Id,
                SeRealizo = true,
                NombreRutina = sesion.NombreRutinaHistorial, // snapshot guardado en el registro
                FechaRealizada = sesion.FechaRealizada,
                DuracionMin = sesion.DuracionMin,
                Ejercicios = sesion.EjerciciosRealizados.Select(er => new EjercicioRealizadoDTO
                {
                    Nombre = er.NombreHistorial,
                    Tipo = er.TipoHistorial,
                    GrupoMuscular = er.GrupoMuscularHistorial,

                    ImagenURL = er.ImagenUrlHistorial, // snapshot
                    Series = er.Series.Select(s => new SerieDTO
                    {
                        Repeticiones = s.Repeticiones,
                        PesoUtilizado = s.PesoUtilizado
                    }).ToList(),

                    Mediciones = er.ValoresMediciones.Select(vm => new MedicionDTO
                    {
                        Nombre = vm.Medicion?.Nombre,  // si es snapshot, puede ser null
                        Unidad = vm.Medicion?.Unidad,
                        Valor = vm.Valor
                    }).ToList()
                }).ToList()
            };

            return dto;
        }
    }
}
