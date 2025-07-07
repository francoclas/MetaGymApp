
using LogicaNegocio.Clases;
using LogicaNegocio.Excepciones;
using LogicaNegocio.Interfaces.Servicios;

using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Extra;
namespace LogicaNegocio.Servicios
{
    public class ServicioUsuario : IUsuarioServicio
    {
        //Acceso repo
        private readonly IRepositorioCliente repoCliente;

        private readonly IRepositorioProfesional repoProfesional;

        private readonly IRepositorioAdmin repoAdmin;

        public ServicioUsuario(IRepositorioCliente rCli, IRepositorioAdmin rAdm,IRepositorioProfesional rPro)
        {
            repoCliente = rCli;
            repoAdmin = rAdm;
            repoProfesional = rPro;
        }
        public void AgregarTelefono(string Usuario, string NumeroNuevo)
        {
            throw new NotImplementedException();
        }

        public void CambiarCorreo(string Usuario, string Correo)
        {
            throw new NotImplementedException();
        }

        public void CambiarNombre(string Usuario, string Nombre)
        {
            throw new NotImplementedException();
        }

        public void CambiarPass(string Usuario, string NuevaPassword, string ConfPassword)
        {
            throw new NotImplementedException();
        }

        public void CrearAdmin(string Ci, string Usuario,string NombreCompleto, string Correo,string Password, string Telefono)
        {
            //Instancio nuevo administrador
            Admin NuevoAdmin = new Admin(Ci,Usuario, NombreCompleto,Password, Correo, Telefono);
            //Valido datos
            NuevoAdmin.Validar();
            //Verifico que no exista usuario
            if (repoAdmin.ExisteUsuario(Usuario))
            {
                throw new UsuarioException("Ya existe un usuario con ese nombre de usuario.");
            }
            //Cargo a repo
            repoAdmin.Agregar(NuevoAdmin);
        }

        public void CrearCliente(string Ci, string NombreUsuario, string NombreCompleto, string Correo, string Password, string Telefono)
        {
            //Instancio nuevo cliente
            Cliente NuevoCliente = new Cliente(Ci,
                NombreUsuario,
                NombreCompleto,
                HashContrasena.Hashear(Password),
                Correo,
                Telefono);
            //Valido datos
            NuevoCliente.Validar();
            //Verificar que no existe
            if (repoCliente.ExisteUsuario(NombreUsuario))
            {
                throw new UsuarioException("Ya existe usuario con ese nombre de usuario");
            }
            //Cargo en el sistema
            repoCliente.Agregar(NuevoCliente);
        }

        public void CrearProfesional(string Ci, string Usuario, string NombreCompleto, string Correo, string Password,string Telefono)
        {
            //Instancio nuevo profesional
            Profesional NuevoProfesional = new Profesional(Ci,
                Usuario,
                NombreCompleto,
                HashContrasena.Hashear(Password),
                Correo,
                Telefono);  
            //Valido datos
            NuevoProfesional.Validar();
            //Verifico que no exista el usuari
            if (repoProfesional.ExisteUsuario(Usuario))
            {
                throw new UsuarioException("Ya existe usuario con ese nombre de usuario");
            }
            //Lo cargo al sistema
            repoProfesional.Agregar(NuevoProfesional);
        }

        public SesionDTO IniciarSesion(LoginDTO login)
        {
            // Reviso en Admin
            var admin = repoAdmin.ObtenerPorUsuario(login.NombreUsuario);
            if (admin != null && HashContrasena.Verificar(admin.Pass, login.Password))
            {
                return new SesionDTO
                {
                    UsuarioId = admin.Id,
                    Nombre = admin.NombreUsuario,
                    NombreCompleto = admin.NombreCompleto,
                    Rol = "Admin"
                };
            }

            // Reviso en Profesional
            var profesional = repoProfesional.ObtenerPorUsuario(login.NombreUsuario);
            if (profesional != null && HashContrasena.Verificar(profesional.Pass, login.Password))
            {
                return new SesionDTO
                {
                    UsuarioId = profesional.Id,
                    Nombre = profesional.NombreUsuario,
                    NombreCompleto = profesional.NombreCompleto,
                    Rol = "Profesional"
                };
            }

            // Reviso en Cliente
            var cliente = repoCliente.ObtenerPorUsuario(login.NombreUsuario);
            if (cliente != null && HashContrasena.Verificar(cliente.Pass, login.Password))
            {
                return new SesionDTO
                {
                    UsuarioId = cliente.Id,
                    Nombre = cliente.NombreUsuario,
                    NombreCompleto = cliente.NombreCompleto,
                    Rol = "Cliente"
                };
            }

            throw new UsuarioException("Revisar credenciales ingresadas");
        }

        public Cliente IniciarSesionCliente(LoginDTO login)
        {
            Cliente UsuarioCliente = repoCliente.VerificarCredenciales(login.NombreUsuario, login.Password);
            //Valido
            if (UsuarioCliente == null)
            {
                throw new UsuarioException("Verificar credenciales ingresadas.");
            }
            //Devuelvo user
            return UsuarioCliente;
        }
        public void RegistrarCliente(ClienteDTO cliente)
        {
            //Verifico que no exista uno con ese nombre de usuario
            if (repoCliente.ExisteUsuario(cliente.NombreUsuario))
            {
                throw new UsuarioException("Ya existe usuario con ese nombre de usuario");
            }
            //Instancio nuevo cliente
            Cliente Nuevo = new Cliente(cliente.Ci,
                cliente.NombreUsuario,
                cliente.NombreCompleto, 
                HashContrasena.Hashear(cliente.Password),
                cliente.Correo, 
                cliente.Telefono);
            //Valido
            Nuevo.Validar();
            //Agrego desde repositorio
            repoCliente.Agregar(Nuevo);
        }

    }
}
