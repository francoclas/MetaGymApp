using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.EntityFrameworkCore;

namespace LogicaApp.Servicios
{

    public class ServicioProfesional : IProfesionalServicio
    {
        // Repositorio de persistencia para Profesional
        private readonly IRepositorioProfesional _repositorioProfesional;

        // Servicio de extras para resolver Tipos de Atención, etc.
        private readonly IExtraServicio _extraServicio;

        // Inyección de dependencias
        public ServicioProfesional(IRepositorioProfesional repoProfesional, IExtraServicio extraServicio)
        {
            _repositorioProfesional = repoProfesional;
            _extraServicio = extraServicio;
        }

        // Actualización básica

        public void ActualizarProfesional(Profesional profesional)
        {
            // Persiste los cambios del profesional
            _repositorioProfesional.Actualizar(profesional);
        }

        // Especialidades

        public void AgregarEspecialidad(Especialidad especialidad, Profesional profesional)
        {
            // Evita duplicar la especialidad en la colección
            if (!profesional.Especialidades.Any(e => e.Id == especialidad.Id))
            {
                profesional.Especialidades.Add(especialidad);
                _repositorioProfesional.Actualizar(profesional);
            }
        }

        public void EliminarEspecialidad(Especialidad especialidad, Profesional profesional)
        {
            // Busca la especialidad en la colección del profesional
            var existente = profesional.Especialidades.FirstOrDefault(e => e.Id == especialidad.Id);
            if (existente != null)
            {
                // La remueve y persiste
                profesional.Especialidades.Remove(existente);
                _repositorioProfesional.Actualizar(profesional);
            }
        }

        // Publicaciones / Citas (no implementado) Se gestiona en su respectivo servicio

        public void EnviarPublicacion(Publicacion publicacion)
        {
            // Pendiente de implementación
            throw new NotImplementedException();
        }

        public void GenerarCita(Cita cita)
        {
            // Pendiente de implementación
            throw new NotImplementedException();
        }

        public void RechazarCita(Cita cita)
        {
            // Pendiente de implementación
            throw new NotImplementedException();
        }

        public void RegistrarProfesional(Profesional profesional)
        {
            // Pendiente de implementación
            throw new NotImplementedException();
        }

        // Consultas de Profesional

        public List<int> ObtenerEspecialidadesProfesional(int profesionalId)
        {
            // Obtiene el profesional y devuelve solo los Ids de sus especialidades
            Profesional pro = _repositorioProfesional.ObtenerPorId(profesionalId);
            return pro.Especialidades.Select(x => x.Id).ToList();
        }

        public Profesional ObtenerProfesional(int id)
        {
            // Trae un profesional por Id 
            return _repositorioProfesional.ObtenerPorId(id);
        }

        public List<Profesional> ObtenerTodos()
        {
            // Lista de todos los profesionales
            return _repositorioProfesional.ObtenerTodos().ToList();
        }

        // Tipos de atención

        public void AsignarTiposAtencion(int profesionalId, List<int> tipoAtencionIds)
        {
            // Reemplaza la colección completa de tipos de atención por los enviados
            var profesional = ObtenerProfesional(profesionalId);
            if (profesional == null) throw new Exception("Profesional no encontrado.");

            var tipos = _extraServicio.ObtenerTiposAtencionPorIds(tipoAtencionIds);
            profesional.TiposAtencion = tipos;

            _repositorioProfesional.Actualizar(profesional);
        }

        public void AgregarTipoAtencion(TipoAtencion tipo, Profesional profesional)
        {
            if (!profesional.TiposAtencion.Contains(tipo))
            {
                profesional.TiposAtencion.Add(tipo);
                _repositorioProfesional.Actualizar(profesional);
            }
        }

        public void EliminarTipoAtencion(int profesionalId, int tipoAtencionId)
        {
            var profesional = ObtenerProfesional(profesionalId);
            if (profesional == null) throw new Exception("Profesional no encontrado.");

            var tipo = profesional.TiposAtencion.FirstOrDefault(t => t.Id == tipoAtencionId);
            if (tipo != null)
            {
                profesional.TiposAtencion.Remove(tipo);
                _repositorioProfesional.Actualizar(profesional);
            }
        }

        public List<int> ObtenerTiposAtencionProfesional(int profesionalId)
        {
            // A diferencia de ObtenerEspecialidadesProfesional, aquí se valida null y se lanza una excepción explícita
            Profesional profe = _repositorioProfesional.ObtenerPorId(profesionalId);

            if (profe == null)
                throw new Exception("No se encontró el profesional.");

            return profe.TiposAtencion.Select(ta => ta.Id).ToList();
        }

        public List<EspecialidadDTO> ObtenerEspecialidadesProfesionalDTO(int profesionalId)
        {
            Profesional profe = _repositorioProfesional.ObtenerPorId(profesionalId);

            List<EspecialidadDTO> salida = new List<EspecialidadDTO>();
            foreach (var item in profe.Especialidades)
            {
                EspecialidadDTO aux = new EspecialidadDTO
                {
                    Id = item.Id,
                    DescripcionEspecialidad = item.DescripcionEspecialidad,
                    NombreEspecialidad = item.NombreEspecialidad,
                    TipoAtenciones = new List<TipoAtencionDTO>()
                };

                if (item.TiposAtencion != null)
                {
                    foreach (TipoAtencion tp in item.TiposAtencion)
                    {
                        aux.TipoAtenciones.Add(new TipoAtencionDTO
                        {
                            EspecialidadId = aux.Id,
                            Desc = tp.Descripcion,
                            Nombre = tp.Nombre,
                            Id = tp.Id
                        });
                    }
                }

                salida.Add(aux);
            }
            return salida;
        }
    }
}
