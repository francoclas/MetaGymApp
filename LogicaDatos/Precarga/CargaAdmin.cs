using LogicaNegocio.Clases;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDatos.Precarga
{
    public static class CargaAdmin
    {
        public static void CargarAdminBase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContextApp>();

            if (!context.Administradores.Any(a => a.NombreUsuario == "admin"))
            {
                var hasher = new PasswordHasher<object>();
                var admin = new Admin
                {
                    NombreUsuario = "admin",
                    NombreCompleto = "Administrador General",
                    CI = "12345678",
                    Correo = "admin@admin.com",
                    Telefono = "099999999",
                    Pass = hasher.HashPassword(null, "gymmeta3220")
                };

                context.Administradores.Add(admin);
                context.SaveChanges();
            }
        }
    }
}
