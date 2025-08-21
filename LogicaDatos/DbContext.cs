using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaDatos.Migrations;

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
        public DbSet<TipoAtencion> TipoAtenciones { get; set; }
        public DbSet<AgendaProfesional> AgendaProfesionales { get; set; }
        //Ejercicios
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }  
        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }
        public DbSet<RutinaAsignada> RutinasAsignadas { get; set; }        
        public DbSet<SesionRutina> SesionesRutina { get; set; }          
        public DbSet<EjercicioRealizado> EjercicioRealizadosPorClientes { get; set; }
        public DbSet<SerieRealizada> SeriesParaEjerciciosDeCliente { get; set; }
        public DbSet<Medicion> Mediciones { get; set; }
        public DbSet<ValorMedicion> MedicionesEjercicio { get; set; }
        //Publicaciones
        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<LikePublicacion> LikePublicaciones { get; set; }
        public DbSet<LikeComentario> LikeComentarios { get; set; }

        //Extras
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Establecimiento> Establecimientos { get; set; }
        //Imagenes y video
        public DbSet<Media> Medias { get; set; }
        //Notificaciones 
        public DbSet<Notificacion> Notificaciones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Enum de estadocitas
            modelBuilder.Entity<Cita>()
           .Property(c => c.Estado)
           .HasConversion<string>();
                modelBuilder.Entity<Media>()
                .Property(m => m.Tipo)
                .HasConversion<string>();
                modelBuilder.Entity<Media>()
                .Property(m => m.TipoEntidad)
                .HasConversion<string>();

                modelBuilder.Entity<Ejercicio>()
                .HasOne(e => e.Profesional)
                .WithMany(p => p.Ejercicios)
                .HasForeignKey(e => e.ProfesionalId)
                .OnDelete(DeleteBehavior.Restrict);
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
            modelBuilder.Entity<LikePublicacion>()
                .HasOne(lp => lp.Publicacion)
                .WithMany(p => p.Likes)
                .HasForeignKey(lp => lp.PublicacionId);
     
            modelBuilder.Entity<LikeComentario>()
                .HasOne(lc => lc.Comentario)
                .WithMany(c => c.Likes)
                .HasForeignKey(lc => lc.ComentarioId);
            modelBuilder.Entity<ValorMedicion>()
                .HasOne(vm => vm.Medicion)
                .WithMany()
                .HasForeignKey(vm => vm.MedicionId)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<ValorMedicion>()
                .HasOne(vm => vm.EjercicioRealizado)
                .WithMany(er => er.ValoresMediciones)
                .HasForeignKey(vm => vm.EjercicioRealizadoId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SesionRutina>()
                .HasOne(sr => sr.Cliente)
                .WithMany(c => c.Entrenamientos)
                .HasForeignKey(sr => sr.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RutinaEjercicio>()
                .HasOne(re => re.Rutina)
                .WithMany(r => r.Ejercicios)
                .HasForeignKey(re => re.RutinaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RutinaEjercicio>()
                .HasOne(re => re.Ejercicio)
                .WithMany(e => e.RutinaEjercicios)
                .HasForeignKey(re => re.EjercicioId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RutinaAsignada>()
                .HasOne(ra => ra.Rutina)
                .WithMany(r => r.Asignaciones)
                .HasForeignKey(ra => ra.RutinaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SesionRutina>()
                .HasOne(sr => sr.RutinaAsignada)
                .WithMany(ra => ra.Sesiones)
                .HasForeignKey(sr => sr.RutinaAsignadaId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<EjercicioRealizado>()
                .HasOne(er => er.SesionRutina)
                .WithMany(sr => sr.EjerciciosRealizados)
                .HasForeignKey(er => er.SesionRutinaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SerieRealizada>()
                .HasOne(s => s.EjercicioRealizado)
                .WithMany(er => er.Series)
                .HasForeignKey(s => s.EjercicioRealizadoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ValorMedicion>()
                .HasOne(vm => vm.EjercicioRealizado)
                .WithMany(er => er.ValoresMediciones)
                .HasForeignKey(vm => vm.EjercicioRealizadoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EjercicioRealizado>()
                .HasOne(er => er.Ejercicio)
                .WithMany()
                .HasForeignKey(er => er.EjercicioId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SesionRutina>()
                .HasOne(sr => sr.Cliente)
                .WithMany(c => c.Entrenamientos)
                .HasForeignKey(sr => sr.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    }

