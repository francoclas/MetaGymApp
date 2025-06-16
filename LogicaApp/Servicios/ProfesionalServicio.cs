using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ProfesionalServicio : IProfesionalServicio

    {
        private readonly IRepositorioProfesional _repoProfesional;

        public ProfesionalServicio (IRepositorioProfesional repoProfesional)
        {
            _repoProfesional = repoProfesional;
        }
        public void ActualizarProfesional(Profesional profesional)
        {
            //Mando el nuevo profesional a la base
            _repoProfesional.Actualizar(profesional);
        }
        public void AgregarEspecialidad(Especialidad especialidad, Profesional profesional)
        {
            //Verifico que no tenga la especialidad
            if (!profesional.Especialidades.Any(e => e.Id == especialidad.Id))
            {
                //La agrego y actualizo
                profesional.Especialidades.Add(especialidad);
                _repoProfesional.Actualizar(profesional);
            }
        }
        public void EliminarEspecialidad(Especialidad especialidad, Profesional profesional)
        {
            //Obtengo la especialidad
            var existente = profesional.Especialidades.FirstOrDefault(e => e.Id == especialidad.Id);
            //Verifico que la tenga asignada
            if (existente != null)
            {
                //La elimino
                profesional.Especialidades.Remove(existente);
                _repoProfesional.Actualizar(profesional);
            }
        }

        public void EnviarPublicacion(Publicacion publicacion)
        {
            throw new NotImplementedException();
        }

        public void GenerarCita(Cita cita)
        {
            throw new NotImplementedException();
        }

        public List<int> ObtenerEspecialidadesProfesional(int profesionalId)
        {
            //Obtengo pro
            Profesional pro = _repoProfesional.ObtenerPorId(profesionalId);
            return pro.Especialidades.Select(x => x.Id).ToList();
        }

        public Profesional ObtenerProfesional(int id)
        {
            return _repoProfesional.ObtenerPorId(id);
        }

        public List<Profesional> ObtenerTodos()
        {
            return _repoProfesional.ObtenerTodos().ToList();
        }

        public void RechazarCita(Cita cita)
        {
            throw new NotImplementedException();
        }

        public void RegistrarProfesional(Profesional profesional)
        {
            throw new NotImplementedException();
        }
    }
}
