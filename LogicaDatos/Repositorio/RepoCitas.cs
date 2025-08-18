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
    public class RepoCitas : IRepositorioCita
    {
        private readonly DbContextApp _context;

        public RepoCitas (DbContextApp context)
        {
            _context = context;
        }
        public void Actualizar(Cita entidad)
        {
            _context.Update(entidad);
            _context.SaveChanges();
        }

        public void Agregar(Cita entidad)
        {
            _context.Add(entidad);
            _context.SaveChanges();
        }

        public List<Cita> BuscarPorTextoConclusion(string texto)
        {
            return _context.Citas
                .Where(c => EF.Functions.Like(c.Conclusion, $"%{texto}%"))
                .ToList();
        }

        public List<Cita> BuscarPorTextoDescripcion(string texto)
        {
                return _context.Citas
                .Where(c => EF.Functions.Like(c.Descripcion, $"%{texto}%"))
                .ToList();
        }

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

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

        public List<Cita> ObtenerPorProfesional(int profesionalId)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(C => C.Profesional)
                .Include(C => C.TipoAtencion)
                .Where(c => c.ProfesionalId == profesionalId)
                .ToList();
        }

        public List<Cita> ObtenerPorEstado(EstadoCita estado)
        {
            return _context.Citas
                .Where(c => c.Estado == estado)
                .ToList();
        }

        public List<Cita> ObtenerEntreFechas(DateTime desde, DateTime hasta)
        {
            return _context.Citas
                .Where(c => c.FechaAsistencia >= desde && c.FechaAsistencia <= hasta)
                .ToList();
        }

        public List<Cita> ObtenerPorClienteYEstado(int clienteId, EstadoCita estado)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(c => c.Especialidad)
                .Include(c => c.Establecimiento)
                .Where(c => c.ClienteId == clienteId && c.Estado == estado)
                .ToList();
        }

        public List<Cita> ObtenerPorProfesionalYEstado(int profesionalId, EstadoCita estado)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(c => c.Especialidad)
                .Include(c=> c.TipoAtencion)
                .Include(c => c.Establecimiento)
                .Where(c => c.ProfesionalId == profesionalId && c.Estado == estado)
                .ToList();
        }

        public IEnumerable<Cita> ObtenerTodos()
        {
            return _context.Citas.ToList();

        }

        public Cita ObtenerPorId(int id)
       {
            return _context.Citas
                .Include(C=>C.Cliente)
                .Include(C=>C.Profesional)
                .Include(C=> C.Especialidad)
                .Include(C=> C.TipoAtencion)
                .Include(C=> C.Establecimiento)
                    .ThenInclude(e => e.Media)
                .FirstOrDefault(c => c.Id == id);
        }

        public bool ExisteCita(Cita cita)
        {
            return _context.Citas.Contains(cita);
        }

        public List<Cita> BuscarPorEstado(EstadoCita estado)
        {
            return _context.Citas
                .Include(C => C.Cliente)
                .Include(c => c.Especialidad)
                .Include(c => c.Establecimiento)
                .Where(c=> c.Estado == estado)
                .ToList();
        }

        public List<Cita> ObtenerHabilitadasParaProfesional(int profesionalid)
        {
            throw new NotImplementedException();
        }
    }
}
