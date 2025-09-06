using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using Microsoft.EntityFrameworkCore;

namespace LogicaDatos.Repositorio
{
    // Repositorio EF Core para Citas
    // Maneja consultas y operaciones básicas sobre la entidad Cita
    public class RepoCitas : IRepositorioCita
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext
        public RepoCitas(DbContextApp context)
        {
            _context = context;
        }

        // Update de una cita existente (entidad debe estar trackeada o se adjunta)
        public void Actualizar(Cita entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        // Alta de nueva cita
        public void Agregar(Cita entidad)
        {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        // Búsqueda por texto en la conclusión
        public List<Cita> BuscarPorTextoConclusion(string texto)
        {
            return _context.Citas
                .Where(c => EF.Functions.Like(c.Conclusion, $"%{texto}%"))
                .ToList();
        }

        // Búsqueda por texto en la descripción 
        public List<Cita> BuscarPorTextoDescripcion(string texto)
        {
            return _context.Citas
            .Where(c => EF.Functions.Like(c.Descripcion, $"%{texto}%"))
            .ToList();
        }

        // Borrado por Id (no implementado)
        public void Eliminar(int id)
        {

            throw new NotImplementedException();
        }

        // Historial por cliente (incluye navegación necesaria para la vista)
        public List<Cita> ObtenerPorCliente(int clienteId)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(C => C.Profesional)
                .Include(C => C.Especialidad)
                .Include(C => C.TipoAtencion)
                .Include(C => C.Establecimiento)
                    .ThenInclude(e => e.Media)
                .Where(c => c.ClienteId == clienteId)
                .ToList();
        }

        // Historial por profesional (con cliente y tipo de atención)
        public List<Cita> ObtenerPorProfesional(int profesionalId)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(C => C.Profesional)
                .Include(C => C.TipoAtencion)
                .Where(c => c.ProfesionalId == profesionalId)
                .ToList();
        }

        // Todas las citas por estado (consulta simple)
        public List<Cita> ObtenerPorEstado(EstadoCita estado)
        {
            return _context.Citas
                .Where(c => c.Estado == estado)
                .ToList();
        }

        // Rango de fechas 
        public List<Cita> ObtenerEntreFechas(DateTime desde, DateTime hasta)
        {
            return _context.Citas
                .Where(c => c.FechaAsistencia >= desde && c.FechaAsistencia <= hasta)
                .ToList();
        }

        // Citas de un cliente filtradas por estado 
        public List<Cita> ObtenerPorClienteYEstado(int clienteId, EstadoCita estado)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(c => c.Especialidad)
                .Include(c => c.Establecimiento)
                .Where(c => c.ClienteId == clienteId && c.Estado == estado)
                .ToList();
        }

        // Citas de un profesional por estado (
        public List<Cita> ObtenerPorProfesionalYEstado(int profesionalId, EstadoCita estado)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(c => c.Especialidad)
                .Include(c => c.TipoAtencion)
                .Include(c => c.Establecimiento)
                .Include(c => c.Profesional)
                    .ThenInclude(p => p.Agendas)
                .Where(c => c.ProfesionalId == profesionalId && c.Estado == estado)
                .ToList();
        }

        // Listado completo 
        public IEnumerable<Cita> ObtenerTodos()
        {
            return _context.Citas.ToList();
        }

        // Detalle de una cita por Id, con relaciones necesarias para mostrarla
        public Cita ObtenerPorId(int id)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(C => C.Profesional)
                .Include(C => C.Especialidad)
                .Include(C => C.TipoAtencion)
                .Include(C => C.Establecimiento)
                    .ThenInclude(e => e.Media)
                .FirstOrDefault(c => c.Id == id);
        }

        // Verifica si la entidad ya está presente
        public bool ExisteCita(Cita cita)
        {
            return _context.Citas.Contains(cita);
        }

        // Citas por estado con datos mínimos de navegación
        public List<Cita> BuscarPorEstado(EstadoCita estado)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(c => c.Especialidad)
                .Include(c => c.Establecimiento)
                .Where(c => c.Estado == estado)
                .ToList();
        }

        //Sin implementar
        public List<Cita> ObtenerHabilitadasParaProfesional(int profesionalid)
        {
            throw new NotImplementedException();
        }
    }
}