using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
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

        public ServicioRutina(IRepositorioRutina repositorioRutina,IRepositorioEjercicio repositorio)
        {
            this.repositorioRutina = repositorioRutina;
            this.repositorioEjercicio = repositorio;
        }

        public void AsignarRutina(Rutina rutina, Cliente cliente)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public EjercicioDTO ObtenerEjercicioDTOId(int id)
        {
            Ejercicio ejercicio = repositorioEjercicio.ObtenerPorId(id);
            EjercicioDTO e = new EjercicioDTO
            {
                Id = ejercicio.Id,
                ProfesionalId = ejercicio.ProfesionalId,
                Nombre = ejercicio.Nombre,
                Tipo = ejercicio.Tipo,
                GrupoMuscular = ejercicio.GrupoMuscular,
                Medias = ejercicio.Medias
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

        public List<Rutina> ObtenerRutinasProfesional(int profesionalId)
        {
           return repositorioRutina.ObtenerPorProfesional(profesionalId);
        }

        public List<Rutina> ObtenerTodasRutinas()
        {
            throw new NotImplementedException();
        }

        public List<EjercicioDTO> ObtenerTodosEjercicios()
        {
            return MapeoEjercicioDTO(repositorioEjercicio.ObtenerTodos().ToList());
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
    }
}
