
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
        public void CambiarTelefono(int IdCliente, string Usuario, string NumeroNuevo)
        {
            Cliente cliente = repoCliente.ObtenerPorId(IdCliente);

            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");

            cliente.Telefono = NumeroNuevo;
            repoCliente.Actualizar(cliente);
            repoCliente.GuardarCambios();
        }

        public void CambiarCorreo(int IdCliente, string Usuario, string Correo)
        {
            Cliente cliente = repoCliente.ObtenerPorId(IdCliente);

            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");
            if(!FuncionesAuxiliares.EsCorreoValido(Correo))
                throw new Exception("Verifique el correo ingresado.");

            cliente.Correo = Correo;
            repoCliente.Actualizar(cliente);
            repoCliente.GuardarCambios();
        }

        public void CambiarNombre(int IdCliente, string Usuario, string Nombre)
        {
            Cliente cliente = repoCliente.ObtenerPorId(IdCliente);

            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");

            cliente.NombreCompleto = Nombre;
            repoCliente.Actualizar(cliente);
            repoCliente.GuardarCambios();
        }

        public void CambiarPass(int IdCliente, string Usuario, string NuevaPassword, string ConfPassword)
        {
            if (NuevaPassword != ConfPassword)
                throw new Exception("Las contraseñas no coinciden.");
            if (!FuncionesAuxiliares.EsContrasenaValida(NuevaPassword))
                throw new Exception("Pruebe con otra contraseña");

            Cliente cliente = repoCliente.ObtenerPorId(IdCliente);
            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");

            cliente.Pass = HashContrasena.Hashear(NuevaPassword);
            repoCliente.Actualizar(cliente);
            repoCliente.GuardarCambios();
        }

        public void CrearAdmin(string Ci, string Usuario,string NombreCompleto, string Correo,string Password, string Telefono)
        {
            //Instancio nuevo administrador
            Admin NuevoAdmin = new Admin(Ci,Usuario, NombreCompleto,Password, Correo, Telefono);
            //Valido datos
            NuevoAdmin.Validar();
            //Verifico que no exista usuario
            VerificarUsuarioRepetido(Usuario, Correo);
            ExisteCI(Ci);
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
            VerificarUsuarioRepetido(NombreUsuario, Correo);
            ExisteCI(Ci);
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
            VerificarUsuarioRepetido(Usuario, Correo);
            ExisteCI(Ci);
            //Lo cargo al sistema
            repoProfesional.Agregar(NuevoProfesional);
        }

        public SesionDTO IniciarSesion(LoginDTO login)
        {
            // Reviso en Admin
            var admin = repoAdmin.ObtenerPorUsuario(login.NombreUsuario);
            if (admin != null && HashContrasena.Verificar(admin.Pass, login.Password))
            {
                if (!admin.UsuarioActivo)
                    throw new UsuarioException("Tu cuenta de administrador está deshabilitada.");

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
                if (!profesional.UsuarioActivo)
                    throw new UsuarioException("Tu cuenta de profesional está deshabilitada.");

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
                if (!cliente.UsuarioActivo)
                    throw new UsuarioException("Tu cuenta de cliente está deshabilitada.");

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

        public SesionDTO IniciarSesionCliente(LoginDTO login)
        {
            Cliente cliente = repoCliente.ObtenerPorUsuario(login.NombreUsuario);

            if (cliente != null && HashContrasena.Verificar(cliente.Pass, login.Password))
            {
                if (!cliente.UsuarioActivo)
                    throw new UsuarioException("Tu cuenta de cliente está deshabilitada.");

                return new SesionDTO
                {
                    Nombre = cliente.NombreUsuario,
                    NombreCompleto = cliente.NombreCompleto,
                    UsuarioId = cliente.Id,
                    Rol = "Cliente"
                };
            }

            throw new UsuarioException("Revisar credenciales ingresadas");
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
            VerificarUsuarioRepetido(cliente.NombreUsuario, cliente.Correo);
            //verifico ci
            ExisteCI(cliente.Ci);
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
        private void VerificarUsuarioRepetido(string NombreUsuario,string Correo)
        {
            if (repoCliente.ExisteUsuario(NombreUsuario))
                throw new UsuarioException("Intente con otro usuario");
            if (repoAdmin.ExisteUsuario(NombreUsuario))
                throw new UsuarioException("Intente con otro usuario");
            if (repoProfesional.ExisteUsuario(NombreUsuario))
                throw new UsuarioException("Intente con otro usuario");
            if (repoCliente.ExisteCorreo(Correo)) 
                throw new UsuarioException("Intente con otro correo");
            if (repoAdmin.ExisteCorreo(Correo))
                throw new UsuarioException("Intente con otro correo");
            if (repoProfesional.ExisteCorreo(Correo))
                throw new UsuarioException("Intente con otro correo");
        }
        private void ExisteCI(string CI)
        {
            if(repoCliente.ExisteCI(CI))
                throw new UsuarioException("Ya existe usuario con esa CI");
            if (repoAdmin.ExisteCI(CI))
                throw new UsuarioException("Ya existe usuario con esa CI");
            if (repoProfesional.ExisteCI(CI))
                throw new UsuarioException("Ya existe usuario con esa CI");
        }
        private UsuarioGenericoDTO MapeoClienteUsuarioDTO(Cliente cliente)
        {
            return new UsuarioGenericoDTO
            {
                Rol = "Cliente",
                Id = cliente.Id,
                Ci = cliente.CI,
                Nombre = cliente.NombreCompleto,
                Correo = cliente.Correo,
                Medias = mediaServicio.ObtenerImagenesUsuario(Enum_TipoEntidad.Cliente, cliente.Id),
                Telefono = cliente.Telefono,
                Perfil = mediaServicio.ObtenerImagenPerfil(Enum_TipoEntidad.Cliente, cliente.Id),
                UsuarioActivo = cliente.UsuarioActivo
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
                Medias = mediaServicio.ObtenerImagenesUsuario(Enum_TipoEntidad.Admin, admin.Id),
                Telefono = admin.Telefono,
                Perfil = mediaServicio.ObtenerImagenPerfil(Enum_TipoEntidad.Admin, admin.Id),
                UsuarioActivo = admin.UsuarioActivo
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
                Medias = mediaServicio.ObtenerImagenesUsuario(Enum_TipoEntidad.Profesional, profesional.Id),
                Telefono = profesional.Telefono,
                Perfil = mediaServicio.ObtenerImagenPerfil(Enum_TipoEntidad.Profesional, profesional.Id),
                UsuarioActivo = profesional.UsuarioActivo
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
                    Cliente cliente = repoCliente.ObtenerPorId(dto.Id);
                    cliente.NombreCompleto = dto.Nombre;
                    cliente.Correo = dto.Correo;
                    cliente.Telefono = dto.Telefono;
                    repoCliente.GuardarCambios();
                    break;

                case "Profesional":
                    Profesional profesional = repoProfesional.ObtenerPorId(dto.Id);
                    profesional.NombreCompleto = dto.Nombre;
                    profesional.Correo = dto.Correo;
                    profesional.Telefono = dto.Telefono;
                    repoProfesional.GuardarCambios();
                    break;

                case "Admin":
                    Admin admin = repoAdmin.ObtenerPorId(dto.Id);
                    admin.NombreCompleto = dto.Nombre;
                    admin.Correo = dto.Correo;
                    admin.Telefono = dto.Telefono;
                    repoAdmin.GuardarCambios();
                    break;

                default:
                    throw new Exception("Rol desconocido.");
            }
        }
 

        public void DeshabilitarUsuario(int usuarioId, string rol, string password)
        {
            switch (rol)
            {
                case "Admin":
                    Admin admin = repoAdmin.ObtenerPorId(usuarioId);
                    if (admin != null && HashContrasena.Verificar(admin.Pass, password))
                    {
                        admin.UsuarioActivo = false;
                        repoAdmin.GuardarCambios();
                    }
                    else
                    {
                        throw new Exception("Verificar contraseña ingresada.");
                    }
                    break;
                case "Profesional":
                    Profesional profesional = repoProfesional.ObtenerPorId(usuarioId);
                    if (profesional != null && HashContrasena.Verificar(profesional.Pass, password))
                    {
                        profesional.UsuarioActivo = false;
                        repoProfesional.GuardarCambios();
                    }
                    else
                    {
                        throw new Exception("Verificar contraseña ingresada.");
                    }
                    break;
                case "Cliente":
                    Cliente cliente = repoCliente.ObtenerPorId(usuarioId);
                    if (cliente != null && HashContrasena.Verificar(cliente.Pass, password))
                    {
                        cliente.UsuarioActivo = false;
                        repoCliente.GuardarCambios();
                    }
                    else
                    {
                        throw new Exception("Verificar contraseña ingresada.");
                    }
                    break;
                default:
                    throw new Exception ("Rol desconocido.");
            }

        }

    }
}
