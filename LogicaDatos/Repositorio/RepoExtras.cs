using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LogicaDatos.Repositorio
{
    public class RepoExtras : IRepositorioExtra
    {
        private readonly DbContextApp _context;

        public RepoExtras(DbContextApp context)
        {
            _context = context;
        }
        public void AltaEspecialidad(Especialidad Nuevo)
        {
            _context.Especialidades.Add(Nuevo);
            _context.SaveChanges();
        }
        public void AltaEstablecimiento(Establecimiento Nuevo)
        {
            _context.Establecimientos.Add(Nuevo);
            _context.SaveChanges();
        }

        public void AltaMedia(Media nueva)
        {
            _context.Medias.Add(nueva); 
        }

        public List<Especialidad> BuscarEspecialidad(string Nombre)
        {
            return _context.Especialidades.Where(e => e.NombreEspecialidad.ToLower().Contains(Nombre.ToLower())).ToList();

        }
        public List<Establecimiento> BuscarEstablecimiento(string Nombre)
        {
            return _context.Establecimientos.Where(e => e.Nombre.ToLower().Contains(Nombre.ToLower())).ToList();
        }

        public List<Especialidad> ListarEspecialidades()
        {
            return _context.Especialidades.ToList();
        }

        public List<Establecimiento> ListarEstablecimientos()
        {
            return _context.Establecimientos.Include(e => e.Media)
            .ToList();
        }

        public void ModificarEspecialidad(Especialidad EspecialidadMod)
        {
            _context.Especialidades.Update(EspecialidadMod);
            _context.SaveChanges();
        }
        public void ModificarEstablecimiento(Establecimiento EstablecimientoMod)
        {
            _context.Establecimientos.Update(EstablecimientoMod);
            _context.SaveChanges();
        }
        public Especialidad ObtenerEspecialidadId(int Id)
        {
            return _context.Especialidades.FirstOrDefault(E => E.Id == Id);
        }

        public Establecimiento ObtenerEstablecimientoId(int Id)
        {
            return _context.Establecimientos.Include(e => e.Media).FirstOrDefault(E => E.Id == Id);
        }
        public void GuardarCambios()
        {
            _context.SaveChanges();
        }
    }
}
