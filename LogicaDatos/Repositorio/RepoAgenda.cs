using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;

namespace LogicaDatos.Repositorio
{
    // Repositorio EF Core para la agenda laboral de cada profesional
    public class RepoAgenda : IRepositorioAgenda
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext
        public RepoAgenda(DbContextApp context)
        {
            _context = context;
        }

        // Alta de una franja/jornada de trabajo
        public void RegistrarAgenda(AgendaProfesional agenda)
        {
            _context.AgendaProfesionales.Add(agenda);
            _context.SaveChanges();
        }

        // Todas las franjas de un profesional
        public List<AgendaProfesional> ObtenerAgendaDelProfesional(int profesionalId)
        {
            return _context.AgendaProfesionales
                .Where(a => a.ProfesionalId == profesionalId)
                .ToList();
        }

        // Eliminar una franja por Id (si existe)
        public void EliminarAgenda(int agendaId)
        {
            var entidad = _context.AgendaProfesionales.Find(agendaId);
            if (entidad != null)
            {
                _context.AgendaProfesionales.Remove(entidad);
                _context.SaveChanges();
            }
        }

        // Traer una franja puntual
        public AgendaProfesional ObtenerPorId(int id)
        {
            return _context.AgendaProfesionales.Find(id);
        }

        // Guardar pendientes (por si operás varias cosas antes)
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }

        // Update de una franja existente
        public void ActualizarAgenda(AgendaProfesional agenda)
        {
            _context.AgendaProfesionales.Update(agenda);
            _context.SaveChanges();
        }

        // Verifica superposición de horarios para un día dado
        // Devuelve true si la nueva franja [horaInicio, horaFin] choca con alguna existente
        public bool ExisteAgendaEnHorario(int profesionalId, Enum_DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return _context.AgendaProfesionales.Any(a =>
                a.ProfesionalId == profesionalId &&
                a.Dia == dia &&
                (
                    // inicio cae dentro de una franja existente
                    (horaInicio >= a.HoraInicio && horaInicio < a.HoraFin) ||
                    // fin cae dentro de una franja existente
                    (horaFin > a.HoraInicio && horaFin <= a.HoraFin) ||
                    // la nueva franja cubre completamente a la existente
                    (horaInicio <= a.HoraInicio && horaFin >= a.HoraFin)
                )
            );
        }

        // Todas las franjas de un profesional en un día específico
        public List<AgendaProfesional> BuscarAgendaPorDia(int profesionalId, Enum_DiaSemana dia)
        {
            return _context.AgendaProfesionales
                .Where(a => a.ProfesionalId == profesionalId && a.Dia == dia)
                .ToList();
        }
    }

}