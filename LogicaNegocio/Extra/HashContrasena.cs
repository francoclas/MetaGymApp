using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Extra
{
    public static class HashContrasena 
    {
        private static readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

        public static string Hashear(string contraseñaPlano)
        {
            return _hasher.HashPassword(null, contraseñaPlano);
        }

        public static bool Verificar(string contraseñaHasheada, string contraseñaIngresada)
        {
            var resultado = _hasher.VerifyHashedPassword(null, contraseñaHasheada, contraseñaIngresada);
            return resultado == PasswordVerificationResult.Success;
        }
        //Para generar pass de EF solo precarga
        public static void MostrarHashEnConsola(string pass)
        {
            var hash = Hashear(pass);
            Console.WriteLine($"Hash para \"{pass}\":\n{hash}");
        }
    }
}
