using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioAgenda : IAgendaServicio
    {
        private readonly IRepositorioAgenda _repo;

        public ServicioAgenda(IRepositorioAgenda repo)
        {
            _repo = repo;
        }

        public void RegistrarAgenda(AgendaProfesional agenda)
        {
            if (_repo.ExisteAgendaEnHorario(agenda.ProfesionalId, agenda.Dia, agenda.HoraInicio, agenda.HoraFin))
                throw new Exception("Ya existe una agenda en ese horario para ese día.");
            _repo.RegistrarAgenda(agenda);
        }

        public List<AgendaProfesional> ObtenerAgendaDelProfesional(int profesionalId)
        {
            return _repo.ObtenerAgendaDelProfesional(profesionalId);
        }

        public void EliminarAgenda(int agendaId)
        {
            _repo.EliminarAgenda(agendaId);
        }

        public AgendaProfesional ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }

        public void ActualizarAgenda(AgendaProfesional agenda)
        {
            if (_repo.ExisteAgendaEnHorario(agenda.ProfesionalId, agenda.Dia, agenda.HoraInicio, agenda.HoraFin))
                throw new Exception("Ya existe una agenda en ese horario para ese día.");
            _repo.ActualizarAgenda(agenda);
        }

        public bool ExisteAgendaEnHorario(int profesionalId, Enum_DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return _repo.ExisteAgendaEnHorario(profesionalId, dia, horaInicio, horaFin);
        }

        public List<AgendaProfesional> BuscarAgendaPorDia(int profesionalId, Enum_DiaSemana dia)
        {
            return _repo.BuscarAgendaPorDia(profesionalId, dia);
        }
    }


}
