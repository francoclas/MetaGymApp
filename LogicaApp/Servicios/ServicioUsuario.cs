
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
        private readonly IMediaServicio mediaServicio;

        public ServicioUsuario(IRepositorioCliente rCli, IRepositorioAdmin rAdm,IRepositorioProfesional rPro, IMediaServicio mediaServicio)
        {
            repoCliente = rCli;
            repoAdmin = rAdm;
            repoProfesional = rPro;
            this.mediaServicio = mediaServicio;
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

        public UsuarioGenericoDTO ObtenerUsuarioGenericoDTO(int usuarioId, string rol)
        {
            UsuarioGenericoDTO salida;
            switch (rol)
            {
                case "Admin":
                    return MapeoAdminUsuarioDTO(repoAdmin.ObtenerPorId(usuarioId));
                    break;
                case "Cliente":
                    return MapeoClienteUsuarioDTO(repoCliente.ObtenerPorId(usuarioId));
                    break;

                case "Profesional":
                    return MapeoProfesionalUsuarioDTO(repoProfesional.ObtenerPorId(usuarioId));
                    break;
                default:
                    return null;
                    break;
            }
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
        private UsuarioGenericoDTO MapeoClienteUsuarioDTO(Cliente cliente)
        {
            return new UsuarioGenericoDTO
            {
                Rol = "Cliente",
                Id = cliente.Id,
                Nombre = cliente.NombreCompleto,
                Correo = cliente.Correo,
                Medias = cliente.FotosPerfil,
                Telefono = cliente.Telefono,
                Perfil = cliente.FotosPerfil.FirstOrDefault(f => f.EsFavorito)
            };

        }
        private UsuarioGenericoDTO MapeoAdminUsuarioDTO(Admin admin)
        {
            return new UsuarioGenericoDTO
            {
                Rol = "Admin",
                Id = admin.Id,
                Nombre = admin.NombreCompleto,
                Correo = admin.Correo,
                Medias = admin.FotosPerfil,
                Telefono = admin.Telefono,
                Perfil = admin.FotosPerfil.FirstOrDefault(f => f.EsFavorito)
            };
        }
        private UsuarioGenericoDTO MapeoProfesionalUsuarioDTO(Profesional profesional)
        {
            return new UsuarioGenericoDTO
            {
                Rol = "Profesional",
                Id = profesional.Id,
                Nombre = profesional.NombreCompleto,
                Correo = profesional.Correo,
                Medias = profesional.FotosPerfil,
                Telefono = profesional.Telefono,
                Perfil = profesional.FotosPerfil.FirstOrDefault(f => f.EsFavorito)
            };
        }

        public void AsignarFotoFavorita(int mediaId, Enum_TipoEntidad tipo, int entidadId)
        {
            mediaServicio.AsignarFotoFavorita(mediaId, tipo, entidadId);
        }

        public void GuardarCambiosGenerales(UsuarioGenericoDTO dto)
        {
            switch (dto.Rol)
            {
                case "Cliente":
                    var cliente = repoCliente.ObtenerPorId(dto.Id);
                    cliente.NombreCompleto = dto.Nombre;
                    cliente.Correo = dto.Correo;
                    cliente.Telefono = dto.Telefono;
                    repoCliente.GuardarCambios();
                    break;

                case "Profesional":
                    var profesional = repoProfesional.ObtenerPorId(dto.Id);
                    profesional.NombreCompleto = dto.Nombre;
                    profesional.Correo = dto.Correo;
                    profesional.Telefono = dto.Telefono;
                    repoProfesional.GuardarCambios();
                    break;

                case "Admin":
                    var admin = repoAdmin.ObtenerPorId(dto.Id);
                    admin.NombreCompleto = dto.Nombre;
                    admin.Correo = dto.Correo;
                    admin.Telefono = dto.Telefono;
                    repoAdmin.GuardarCambios();
                    break;

                default:
                    throw new Exception("Rol desconocido.");
            }
        }
    }
}
