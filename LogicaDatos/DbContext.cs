using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using LogicaNegocio.Clases;

namespace LogicaDatos
{
    public class DbContextApp : DbContext
    {
        public DbContextApp(DbContextOptions<DbContextApp> options) :base(options){ }

        //DBSets para cada clase del sistema
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Profesional> Profesionales { get; set; }
        public DbSet<Admin> Administradores { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }  
        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Establecimiento> Establecimientos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            //Rutina ejercicios para repes y series
                modelBuilder.Entity<RutinaEjercicio>()
            .HasKey(re => new { re.RutinaId, re.EjercicioId });

                modelBuilder.Entity<RutinaEjercicio>()
                    .HasOne(re => re.Rutina)
                    .WithMany(r => r.RutinaEjercicios)
                    .HasForeignKey(re => re.RutinaId);

                modelBuilder.Entity<RutinaEjercicio>()
                    .HasOne(re => re.Ejercicio)
                    .WithMany(e => e.RutinaEjercicios)
                    .HasForeignKey(re => re.EjercicioId);
            //Enum de estadocitas
            modelBuilder.Entity<Cita>()
           .Property(c => c.Estado)
           .HasConversion<string>();


        }

    }

}
