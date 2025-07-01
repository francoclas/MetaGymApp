using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IAdminServicio
    {

        void CambiarCI(string UsuarioPeticion, string UsuarioACambiar, string CI);
        void RegistrarProfesional(string Administrador, string Usuario, string Correo, string Password, string ConfPassword);
        void RegistrarAdministrador(string Administrador, string Usuario, string Correo, string Password, string ConfPassword);

        List<Admin> ObtenerTodos();
        Admin ObtenerPorId(int id);
        void ActualizarAdmin(Admin admin);
    }
}
