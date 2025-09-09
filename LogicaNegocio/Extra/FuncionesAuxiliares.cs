using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogicaNegocio.Excepciones;

namespace LogicaNegocio.Extra
{
    public static class FuncionesAuxiliares
    {
        public static void ValidarDatosUsuario(string CI,string Pass,string Correo)
        {
            //Que no esten vacios
            if (string.IsNullOrWhiteSpace(CI)) throw new UsuarioException("CI requerida");
            if (string.IsNullOrWhiteSpace(Pass)) throw new UsuarioException("Contraseña requerida");
            if (string.IsNullOrWhiteSpace(Correo)) throw new UsuarioException("Correo requerido");
            //Validaciones
            if (!EsCorreoValido(Correo))
            {
                throw new UsuarioException("El correo no conforma las normas de seguridad.");
            }
            EsContrasenaValida(Pass);
        }
        //Validaciones de datos
        public static bool EsCorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;
            string aux = correo.Trim('"').Trim();
            string PatronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; ;
            return Regex.IsMatch(aux, PatronCorreo);
        }

        /*
         Para verificar que la contraseña sea valida se necesita que: Tenga una minuscula y una mayuscula,
            un numero y un caracter especial. Tambien que tenga mas de 8 caracteres.
         */
        public static bool EsContrasenaValida(string pass)
        {
            if (string.IsNullOrWhiteSpace(pass))
                throw new UsuarioException("La contraseña no puede estar vacia.");
            if (pass.Length < 8)
                throw new UsuarioException("La contraseña debe tener almenos 8 caracteres.");

            bool tieneMayus = false;
            bool tieneMin = false;
            bool tieneDig = false;

            foreach (char L in pass)
            {
                if (char.IsUpper(L)) tieneMayus = true;
                if (char.IsLower(L)) tieneMin = true;
                if (char.IsDigit(L) || char.IsNumber(L)) tieneDig = true;
            }
            return tieneMayus && tieneMin && tieneDig;
        }
        /*
         Para verificar que el telefono sea valido se necesita que: Solo tenga numeros y este en tre 8 y 9 caracteres,
         */
        public static bool EsTelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;
            if (!telefono.All(char.IsDigit))
                return false;
            return telefono.Length >= 8 && telefono.Length <= 9;
        }
        public static bool EsCedulaValida(string ci)
        {
            if (string.IsNullOrWhiteSpace(ci))
                return false;

            // Solo dígitos
            if (!ci.All(char.IsDigit))
                return false;

            // Longitud válida: 7 u 8 dígitos
            return ci.Length == 7 || ci.Length == 8;
        }
    }
    }

