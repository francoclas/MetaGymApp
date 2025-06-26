using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaDatos
{
    public class DbContextApp : DbContext
    {
        public DbContextApp(DbContextOptions<DbContextApp> options) :base(options){ }

        //DBSets para cada clase del sistema
        //Usuarios
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Profesional> Profesionales { get; set; }
        public DbSet<Admin> Administradores { get; set; }
        //Citas
        public DbSet<Cita> Citas { get; set; }
        //Ejercicios
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }  
        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }
        public DbSet<SesionRutina> RutinasRealizadasClientes { get; set; }
        public DbSet<EjercicioRealizado> EjercicioRealizadosPorClientes { get; set; }
        public DbSet<SerieRealizada> SeriesParaEjerciciosDeCliente { get; set; }

        //Publicaciones
        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        //Extras
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Establecimiento> Establecimientos { get; set; }
        //Imagenes y video
        public DbSet<Media> Medias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Enum de estadocitas
            modelBuilder.Entity<Cita>()
           .Property(c => c.Estado)
           .HasConversion<string>();
            //Enum de tipo multimedia
                modelBuilder.Entity<Media>()
                .Property(m => m.Tipo)
                .HasConversion<string>();

            //Relacion profesionhales tienen sus ejercicios
                modelBuilder.Entity<Ejercicio>()
                .HasOne(e => e.Profesional)
                .WithMany(p => p.Ejercicios)
                .HasForeignKey(e => e.ProfesionalId)
                .OnDelete(DeleteBehavior.Restrict);
            //Relacion publicacion
            modelBuilder.Entity<Publicacion>()
                .HasOne(p => p.AdminCreador)
                .WithMany()
                .HasForeignKey(p => p.AdminCreadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Publicacion>()
                .HasOne(p => p.AdminAprobador)
                .WithMany()
                .HasForeignKey(p => p.AdminAprobadorId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }

}
