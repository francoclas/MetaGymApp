using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioAdmin : IAdminServicio
    {
        private readonly IRepositorioAdmin _repositorioAdmin;

        public ServicioAdmin(IRepositorioAdmin repo)
        {
            _repositorioAdmin = repo;
        }
        public void CambiarCI(string UsuarioPeticion, string UsuarioACambiar, string CI)
        {
            throw new NotImplementedException();
        }

        public List<Admin> ObtenerTodos()
        {
            return _repositorioAdmin.ObtenerTodos().ToList();
        }

        public void RegistrarAdministrador(string Administrador, string Usuario, string Correo, string Password, string ConfPassword)
        {
            throw new NotImplementedException();
        }

        public void RegistrarProfesional(string Administrador, string Usuario, string Correo, string Password, string ConfPassword)
        {
            throw new NotImplementedException();
        }
    }
}
