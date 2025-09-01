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
        private readonly IRepositorioAgenda _repositorioAgenda;

        public ServicioAgenda(IRepositorioAgenda repo)
        {
            _repositorioAgenda = repo;
        }

        public void RegistrarAgenda(AgendaProfesional agenda)
        {
            if (_repositorioAgenda.ExisteAgendaEnHorario(agenda.ProfesionalId, agenda.Dia, agenda.HoraInicio, agenda.HoraFin))
                throw new Exception("Ya existe una agenda en ese horario para ese día.");
            _repositorioAgenda.RegistrarAgenda(agenda);
        }

        public List<AgendaProfesional> ObtenerAgendaDelProfesional(int profesionalId)
        {
            return _repositorioAgenda.ObtenerAgendaDelProfesional(profesionalId);
        }

        public void EliminarAgenda(int agendaId)
        {
            _repositorioAgenda.EliminarAgenda(agendaId);
        }

        public AgendaProfesional ObtenerPorId(int id)
        {
            return _repositorioAgenda.ObtenerPorId(id);
        }

        public void ActualizarAgenda(AgendaProfesional agenda)
        {
            _repositorioAgenda.ActualizarAgenda(agenda);
        }

        public bool ExisteAgendaEnHorario(int profesionalId, Enum_DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return _repositorioAgenda.ExisteAgendaEnHorario(profesionalId, dia, horaInicio, horaFin);
        }

        public List<AgendaProfesional> BuscarAgendaPorDia(int profesionalId, Enum_DiaSemana dia)
        {
            return _repositorioAgenda.BuscarAgendaPorDia(profesionalId, dia);
        }
    }


}
