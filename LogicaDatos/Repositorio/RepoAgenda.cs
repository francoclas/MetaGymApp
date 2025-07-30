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
    public class RepoAgenda : IRepositorioAgenda
    {
        private readonly DbContextApp _context;

        public RepoAgenda(DbContextApp context)
        {
            _context = context;
        }

        public void RegistrarAgenda(AgendaProfesional agenda)
        {
            _context.AgendaProfesionales.Add(agenda);
            _context.SaveChanges();
        }

        public List<AgendaProfesional> ObtenerAgendaDelProfesional(int profesionalId)
        {
            return _context.AgendaProfesionales
                .Where(a => a.ProfesionalId == profesionalId)
                .ToList();
        }

        public void EliminarAgenda(int agendaId)
        {
            var entidad = _context.AgendaProfesionales.Find(agendaId);
            if (entidad != null)
            {
                _context.AgendaProfesionales.Remove(entidad);
                _context.SaveChanges();
            }
        }

        public AgendaProfesional ObtenerPorId(int id)
        {
            return _context.AgendaProfesionales.Find(id);
        }

        public void GuardarCambios()
        {
            _context.SaveChanges();
        }

        public void ActualizarAgenda(AgendaProfesional agenda)
        {
            _context.AgendaProfesionales.Update(agenda);
            _context.SaveChanges();
        }

        public bool ExisteAgendaEnHorario(int profesionalId, Enum_DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return _context.AgendaProfesionales.Any(a =>
                a.ProfesionalId == profesionalId &&
                a.Dia == dia &&
                (
                    (horaInicio >= a.HoraInicio && horaInicio < a.HoraFin) ||
                    (horaFin > a.HoraInicio && horaFin <= a.HoraFin) ||
                    (horaInicio <= a.HoraInicio && horaFin >= a.HoraFin)
                )
            );
        }

        public List<AgendaProfesional> BuscarAgendaPorDia(int profesionalId, Enum_DiaSemana dia)
        {
            return _context.AgendaProfesionales
                .Where(a => a.ProfesionalId == profesionalId && a.Dia == dia)
                .ToList();
        }
    }

}
