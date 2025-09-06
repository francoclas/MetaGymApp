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

        // Inyección del repositorio concreto de Admin
        public ServicioAdmin(IRepositorioAdmin repo)
        {
            _repositorioAdmin = repo;
        }

        // Actualiza un Admin ya existente
        public void ActualizarAdmin(Admin admin)
        {
            _repositorioAdmin.Actualizar(admin);
        }
        //Sin implementar
        public void CambiarCI(string UsuarioPeticion, string UsuarioACambiar, string CI)
        {
            throw new NotImplementedException();
        }

        // Obtiene un Admin por Id (retorna null si no existe)
        public Admin ObtenerPorId(int id)
        {
            return _repositorioAdmin.ObtenerPorId(id);
        }

        // Listado de todos los Admins
        public List<Admin> ObtenerTodos()
        {
            return _repositorioAdmin.ObtenerTodos().ToList();
        }

        // Alta de un nuevo Administrador - Quedo sin utilizarse
        public void RegistrarAdministrador(string Administrador, string Usuario, string Correo, string Password, string ConfPassword)
        {
            throw new NotImplementedException();
        }

        // Alta de un nuevo Profesional realizada por un Admin - Quedo sin utilizarse
        public void RegistrarProfesional(string Administrador, string Usuario, string Correo, string Password, string ConfPassword)
        {
            throw new NotImplementedException();
        }
    }
}