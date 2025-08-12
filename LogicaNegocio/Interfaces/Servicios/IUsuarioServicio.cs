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
        SesionDTO IniciarSesionCliente(LoginDTO login);

        //Registro usuario
        void RegistrarCliente(ClienteDTO cliente);
        //Cambio de contraseña
        void CambiarPass(int IdCliente, string NombreUsuario, string NuevaPassword,string ConfPassword);
        //Modificar datos
        void CambiarTelefono(int IdCliente,string NombreUsuario, string NumeroNuevo);
        void CambiarNombre(int IdCliente, string NombreUsuario, string Nombre);
        void CambiarCorreo(int IdCliente, string NombreUsuario, string Correo);
        void AsignarFotoFavorita(int mediaId, Extra.Enum_TipoEntidad tipo, int entidadId);
        //Acceso administrador
        void CrearAdmin(string Ci, string NombreUsuario,string NombreCompleto, string Correo, string Password,string Telefono);
        void CrearProfesional(string Ci, string NombreUsuario, string NombreCompleto, string Correo, string Password,string Telefono);
        void CrearCliente(string Ci, string NombreUsuario, string NombreCompleto, string Correo, string Password, string Telefono);
        UsuarioGenericoDTO ObtenerUsuarioGenericoDTO(int usuarioId, string rol);
        void GuardarCambiosGenerales(UsuarioGenericoDTO dto);
    }
}
