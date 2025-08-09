using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using Microsoft.EntityFrameworkCore;

namespace LogicaDatos.Repositorio
{
    public class RepoRutinas : IRepositorioRutina
    {
        private readonly DbContextApp _context;

        public RepoRutinas (DbContextApp context)
        {
            _context = context;
        }

        public void Actualizar(Rutina entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        public void Agregar(Rutina entidad)
        {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        public void AsignarRutinaACliente(RutinaAsignada asignacion)
        {
            _context.RutinasAsignadas.Add(asignacion);
            _context.SaveChanges();
        }


        public List<Rutina> BuscarPorNombre(string nombre)
        {
            return _context.Rutinas
                .Where(r => r.NombreRutina.Contains(nombre))
                .ToList();
        }
        public bool ClienteTieneRutinaAsignada(int clienteId, int rutinaId)
        {
            return _context.RutinasAsignadas
                .Any(ra => ra.ClienteId == clienteId && ra.RutinaId == rutinaId);
        }

        public void Eliminar(int id)
        {
            var rutina = _context.Rutinas.Find(id);
            if (rutina != null)
            {
                _context.Rutinas.Remove(rutina);
                _context.SaveChanges();
            }
        }

        public List<RutinaAsignada> ObtenerAsignacionesPorCliente(int clienteId)
        {
            return _context.RutinasAsignadas
                .Include(ra => ra.Rutina)
                .Include(ra => ra.Sesiones)
                .Where(ra => ra.ClienteId == clienteId)
                .ToList();
        }

        public Rutina ObtenerPorId(int id)
        {
            return _context.Rutinas
        .Include(r => r.Ejercicios)
            .ThenInclude(re => re.Ejercicio)
            .ThenInclude(ej => ej.Medias)
        .FirstOrDefault(r => r.Id == id);

        }

        public List<Rutina> ObtenerPorProfesional(int profesionalId)
        {
            return _context.Rutinas
                    .Where(r => r.ProfesionalId == profesionalId)
                    .ToList();
        }

        public List<SesionRutina> ObtenerSesionesPorAsignacion(int rutinaAsignadaId)
        {
            return _context.SesionesRutina
                .Where(sr => sr.RutinaAsignadaId == rutinaAsignadaId)
                .Include(sr => sr.EjerciciosRealizados)
                .ToList();
        }
        public List<RutinaAsignada> ObtenerAsignacionesPorRutina(int rutinaId)
        {
            return _context.RutinasAsignadas
                .Where(ra => ra.RutinaId == rutinaId)
                .ToList();
        }

        public List<SesionRutina> ObtenerSesionesPorCliente(int clienteId)
        {
            return _context.SesionesRutina
                .Include(sr => sr.RutinaAsignada)
                    .ThenInclude(ra => ra.Rutina)
                    .Include(sr => sr.EjerciciosRealizados)
                    .ThenInclude(er => er.Ejercicio)
                    .Include(sr => sr.Cliente)
                .Where(sr => sr.RutinaAsignada.ClienteId == clienteId)
                .ToList();
        }

        public SesionRutina? ObtenerSesionPorId(int sesionId)
        {
            return _context.SesionesRutina
               .Include(sr => sr.RutinaAsignada)
                .ThenInclude(ra => ra.Rutina)
               .Include(sr => sr.EjerciciosRealizados)
               .ThenInclude(er => er.Series)
               .Include(sr => sr.EjerciciosRealizados)
                .ThenInclude(er => er.Ejercicio)
               .FirstOrDefault(sr => sr.Id == sesionId);
        }

        public IEnumerable<Rutina> ObtenerTodos()
        {
            return _context.Rutinas
                .Include(r => r.Profesional)
                .Include(r => r.Ejercicios)
                .ToList();
        }

        public SesionRutina RegistrarSesion(SesionRutina sesion)
        {
            _context.SesionesRutina.Add(sesion);
            _context.SaveChanges();
            return sesion;
        }

        public RutinaAsignada? ObtenerAsignacion(int asignacionId)
        {
            return _context.RutinasAsignadas
                .Include(r => r.Rutina)
                .FirstOrDefault(r => r.Id == asignacionId);
        }
        public void RemoverAsignacion(int rutinaAsignadaId)
        {
            var asignacion = _context.RutinasAsignadas.Find(rutinaAsignadaId);
            if (asignacion != null)
            {
                _context.RutinasAsignadas.Remove(asignacion);
                _context.SaveChanges();
            }
        }
    }
    }
