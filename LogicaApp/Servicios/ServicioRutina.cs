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

namespace LogicaApp.Servicios
{
    public class ServicioRutina : IRutinaServicio
    {
        private readonly IRepositorioRutina repositorioRutina;
        private readonly IRepositorioEjercicio repositorioEjercicio;
        private readonly INotificacionServicio notificacionServicio;

        public ServicioRutina(IRepositorioRutina repositorioRutina,IRepositorioEjercicio repositorio, INotificacionServicio inotificacionServicio)
        {
            this.repositorioRutina = repositorioRutina;
            this.repositorioEjercicio = repositorio;
            this.notificacionServicio = inotificacionServicio;
        }

        public void AsignarRutina(Rutina rutina, Cliente cliente)
        {
            throw new NotImplementedException();
        }

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
                repositorioRutina.AsignarRutinaACliente(nuevaAsignacion);
                notificacionServicio.NotificarRutinaAsignada(clienteId, rutinaId);
            }
        }
        public void ReemplazarAsignaciones(int rutinaId, List<int> nuevosClienteIds)
        {
            var existentes = repositorioRutina.ObtenerAsignacionesPorRutina(rutinaId);

            // Remover todos los actuales
            foreach (var a in existentes)
            {
                repositorioRutina.RemoverAsignacion(a.Id);
            }

            // Asignar los nuevos
            foreach (var id in nuevosClienteIds)
            {
                repositorioRutina.AsignarRutinaACliente(new RutinaAsignada
                {
                    ClienteId = id,
                    RutinaId = rutinaId,
                    FechaAsignacion = DateTime.Now
                });
                notificacionServicio.NotificarRutinaAsignada(id, rutinaId);
            }
        }
        public bool ClienteTieneRutinaAsignada(int clienteId, int rutinaId)
        {
            return repositorioRutina.ClienteTieneRutinaAsignada(clienteId, rutinaId);
        }

        public void DesasignarRutina(Rutina rutina, Cliente cliente)
        {
            throw new NotImplementedException();
        }

        public Rutina GenerarNuevaRutina(Rutina rutina)
        {
            repositorioRutina.Agregar(rutina);
            return rutina;
        }

        public Ejercicio GenerarNuevoEjercicio(Ejercicio ejercicio)
        {
            repositorioEjercicio.Agregar(ejercicio);
            return ejercicio ;
        }

        public void ModificarEjercicio(Ejercicio ejercicio)
        {
            repositorioEjercicio.Actualizar(ejercicio);
        }

        public void ModificarRutina(Rutina rutina)
        {
            repositorioRutina.Actualizar(rutina);
        }

        public EjercicioDTO ObtenerEjercicioDTOId(int id)
        {
            Ejercicio ejercicio = repositorioEjercicio.ObtenerPorId(id);
            if (ejercicio == null) return null;
            EjercicioDTO e = new EjercicioDTO
            {
                Id = ejercicio.Id,
                ProfesionalId = ejercicio.ProfesionalId,
                Nombre = ejercicio.Nombre,
                Tipo = ejercicio.Tipo,
                GrupoMuscular = ejercicio.GrupoMuscular,
                Media = ejercicio.Medias[0],
                Medias = ejercicio.Medias,
                Mediciones = ejercicio.Mediciones,
                Instrucciones = ejercicio.Instrucciones
            };
            return e;

        }

        public Ejercicio ObtenerEjercicioId(int id)
        {
            return repositorioEjercicio.ObtenerPorId(id);
        }

        public List<EjercicioDTO> ObtenerEjerciciosProfesional(int Id)
        {
            return MapeoEjercicioDTO(repositorioEjercicio.ObtenerPorProfesional(Id));
        }

        public List<SesionRutina> ObtenerHistorialClienteDTO(int clienteId)
        {
            return repositorioRutina.ObtenerSesionesPorCliente(clienteId);
        }

        public List<RutinaAsignada> ObtenerRutinasAsignadasCliente(int clienteId)
        {
            return repositorioRutina.ObtenerAsignacionesPorCliente(clienteId);
        }

        public List<Rutina> ObtenerRutinasProfesional(int profesionalId)
        {
           return repositorioRutina.ObtenerPorProfesional(profesionalId);
        }

        public SesionRutina? ObtenerSesionPorId(int sesionId)
        {
            return repositorioRutina.ObtenerSesionPorId(sesionId);
        }

        public List<Rutina> ObtenerTodasRutinas()
        {
            return repositorioRutina.ObtenerTodos().ToList();
        }

        public List<EjercicioDTO> ObtenerTodosEjercicios()
        {
            return MapeoEjercicioDTO(repositorioEjercicio.ObtenerTodos().ToList());
        }

        public SesionRutina RegistrarSesion(SesionRutina sesion)
        {
            if (sesion == null)
                throw new Exception("La sesión no puede ser nula");

            var rutinaAsignada = repositorioRutina.ObtenerAsignacion(sesion.RutinaAsignadaId);
            if (rutinaAsignada == null)
                throw new Exception("No se encontró la rutina asignada");

            if (rutinaAsignada.ClienteId != sesion.ClienteId)
                throw new Exception("No coincide el cliente con la rutina asignada");

            if (sesion.EjerciciosRealizados == null || !sesion.EjerciciosRealizados.Any())
                throw new Exception("Debe registrar al menos un ejercicio");

            foreach (var ej in sesion.EjerciciosRealizados)
            {
                if (ej.SeRealizo)
                {
                    if (ej.Series == null || !ej.Series.Any())
                        throw new Exception($"El ejercicio {ej.EjercicioId} está marcado como realizado pero no tiene series");

                    // Validación de mediciones obligatorias
                    var ejercicioOriginal = repositorioEjercicio.ObtenerPorId(ej.EjercicioId);
                    var obligatorias = ejercicioOriginal.Mediciones.Where(m => m.EsObligatoria).ToList();

                    foreach (var ob in obligatorias)
                    {
                        if (!ej.ValoresMediciones.Any(vm => vm.MedicionId == ob.Id && !string.IsNullOrWhiteSpace(vm.Valor)))
                            throw new Exception($"Falta valor para medición obligatoria '{ob.Nombre}' del ejercicio {ejercicioOriginal.Nombre}");
                    }
                }
            }

            sesion.FechaRealizada = DateTime.Now;
            return repositorioRutina.RegistrarSesion(sesion);
        }
        public List<SesionRutina> ObtenerSesionesPorAsignacion(int rutinaAsignadaId)
        {
            return repositorioRutina.ObtenerSesionesPorAsignacion(rutinaAsignadaId);
        }
        public void RemoverAsignacion(int rutinaAsignadaId)
        {
            repositorioRutina.RemoverAsignacion(rutinaAsignadaId);
        }

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

        public Rutina ObtenerRutinaPorId(int id)
        {
            return repositorioRutina.ObtenerPorId(id);
        }

        public List<RutinaAsignada> ObtenerAsignacionesPorRutina(int rutinaId)
        {
            return repositorioRutina.ObtenerAsignacionesPorRutina(rutinaId);
        }
        public RutinaAsignadaDTO ObtenerDetalleRutinaAsignadaDTO(int rutinaAsignadaId, int clienteId)
        {
            var asignacion = repositorioRutina.ObtenerAsignacionesPorCliente(clienteId)
                .FirstOrDefault(a => a.Id == rutinaAsignadaId && a.ClienteId == clienteId);

            if (asignacion == null) return null;

            var rutina = repositorioRutina.ObtenerPorId(asignacion.RutinaId);
            var sesiones = repositorioRutina.ObtenerSesionesPorAsignacion(rutinaAsignadaId);

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

        public string ObtenerNombreRutina(int idRutina)
        {
            Rutina salida = repositorioRutina.ObtenerPorId(idRutina);

            return salida.NombreRutina;
        }
    }
}
