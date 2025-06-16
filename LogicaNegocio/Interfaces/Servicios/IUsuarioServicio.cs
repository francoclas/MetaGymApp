using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IUsuarioServicio
    {
        //Inicio de sesion

        SesionDTO IniciarSesion(LoginDTO login);
        Cliente IniciarSesionCliente(LoginDTO login);
        Admin IniciarSesionAdmin(string NombreUsuario, string Password);
        Profesional IniciarSesionProfesional(string NombreUsuario, string Password);
        //Registro usuario
        void RegistrarCliente(ClienteDTO cliente);
        //Cambio de contraseña
        void CambiarPass(string NombreUsuario, string NuevaPassword,string ConfPassword);

        //Modificar datos
        void AgregarTelefono(string NombreUsuario, string NumeroNuevo);
        void CambiarNombre(string NombreUsuario, string Nombre);
        void CambiarCorreo(string NombreUsuario, string Correo);
        
        //Acceso administrador
        void CrearAdmin(string Ci, string NombreUsuario,string NombreCompleto, string Correo, string Password,string Telefono);
        void CrearProfesional(string Ci, string NombreUsuario, string NombreCompleto, string Correo, string Password,string Telefono);
        void CrearCliente(string Ci, string NombreUsuario, string NombreCompleto, string Correo, string Password, string Telefono);


    }
}
