
using LogicaNegocio.Clases;
using LogicaNegocio.Excepciones;
using LogicaNegocio.Interfaces.Servicios;

using LogicaDatos.Interfaces.Repos;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Extra;
using System.Text.RegularExpressions;
namespace LogicaNegocio.Servicios
{
    public class ServicioUsuario : IUsuarioServicio
    {
        //Acceso repo
        private readonly IRepositorioCliente _repositorioCliente;
        private readonly IRepositorioProfesional _repositorioProfesional;
        private readonly IRepositorioAdmin _repositorioAdmin;
        private readonly IMediaServicio _mediaServicio;

        public ServicioUsuario(IRepositorioCliente rCli, IRepositorioAdmin rAdm,IRepositorioProfesional rPro, IMediaServicio mediaServicio)
        {
            _repositorioCliente = rCli;
            _repositorioAdmin = rAdm;
            _repositorioProfesional = rPro;
            this._mediaServicio = mediaServicio;
        }
        public void CambiarTelefono(int IdCliente, string Usuario, string NumeroNuevo)
        {
            Cliente cliente = _repositorioCliente.ObtenerPorId(IdCliente);
            if (string.IsNullOrWhiteSpace(NumeroNuevo) ||
            !Regex.IsMatch(NumeroNuevo.Replace(" ", "").Replace("-", ""), @"^\+?\d{8,15}$"))
                throw new ArgumentException("Número de teléfono inválido.");
            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");
            cliente.Telefono = NumeroNuevo;
            _repositorioCliente.Actualizar(cliente);
            _repositorioCliente.GuardarCambios();
        }

        public void CambiarCorreo(int IdCliente, string Usuario, string Correo)
        {
            Cliente cliente = _repositorioCliente.ObtenerPorId(IdCliente);

            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");
            if(!FuncionesAuxiliares.EsCorreoValido(Correo))
                throw new Exception("Verifique el correo ingresado.");

            cliente.Correo = Correo;
            _repositorioCliente.Actualizar(cliente);
            _repositorioCliente.GuardarCambios();
        }

        public void CambiarNombre(int IdCliente, string Usuario, string Nombre)
        {
            Cliente cliente = _repositorioCliente.ObtenerPorId(IdCliente);

            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");

            cliente.NombreCompleto = Nombre;
            _repositorioCliente.Actualizar(cliente);
            _repositorioCliente.GuardarCambios();
        }

        public void CambiarPass(int IdCliente, string Usuario, string NuevaPassword, string ConfPassword)
        {
            if (NuevaPassword != ConfPassword)
                throw new Exception("Las contraseñas no coinciden.");
            if (!FuncionesAuxiliares.EsContrasenaValida(NuevaPassword))
                throw new Exception("Pruebe con otra contraseña");

            Cliente cliente = _repositorioCliente.ObtenerPorId(IdCliente);
            if (cliente == null || cliente.NombreUsuario != Usuario)
                throw new Exception("Cliente no encontrado o usuario inválido.");

            cliente.Pass = HashContrasena.Hashear(NuevaPassword);
            _repositorioCliente.Actualizar(cliente);
            _repositorioCliente.GuardarCambios();
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
            _repositorioAdmin.Agregar(NuevoAdmin);
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
            _repositorioCliente.Agregar(NuevoCliente);
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
            _repositorioProfesional.Agregar(NuevoProfesional);
        }

        public SesionDTO IniciarSesion(LoginDTO login)
        {
            // Reviso en Admin
            var admin = _repositorioAdmin.ObtenerPorUsuario(login.NombreUsuario);
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
            var profesional = _repositorioProfesional.ObtenerPorUsuario(login.NombreUsuario);
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
            var cliente = _repositorioCliente.ObtenerPorUsuario(login.NombreUsuario);
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
            Cliente cliente = _repositorioCliente.ObtenerPorUsuario(login.NombreUsuario);

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
                    return MapeoAdminUsuarioDTO(_repositorioAdmin.ObtenerPorId(usuarioId));
                    break;
                case "Cliente":
                    return MapeoClienteUsuarioDTO(_repositorioCliente.ObtenerPorId(usuarioId));
                    break;

                case "Profesional":
                    return MapeoProfesionalUsuarioDTO(_repositorioProfesional.ObtenerPorId(usuarioId));
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
            _repositorioCliente.Agregar(Nuevo);
        }
        public void VerificarUsuarioRepetido(string NombreUsuario,string Correo)
        {
            if (_repositorioCliente.ExisteUsuario(NombreUsuario))
                throw new UsuarioException("Intente con otro usuario");
            if (_repositorioAdmin.ExisteUsuario(NombreUsuario))
                throw new UsuarioException("Intente con otro usuario");
            if (_repositorioProfesional.ExisteUsuario(NombreUsuario))
                throw new UsuarioException("Intente con otro usuario");
            if (_repositorioCliente.ExisteCorreo(Correo)) 
                throw new UsuarioException("Intente con otro correo");
            if (_repositorioAdmin.ExisteCorreo(Correo))
                throw new UsuarioException("Intente con otro correo");
            if (_repositorioProfesional.ExisteCorreo(Correo))
                throw new UsuarioException("Intente con otro correo");
        }
        public void VerificarCorreoUnico(string correo)
        {
            if (_repositorioCliente.ExisteCorreo(correo))
                throw new UsuarioException("Intente con otro correo");

            if (_repositorioAdmin.ExisteCorreo(correo))
                throw new UsuarioException("Intente con otro correo");

            if (_repositorioProfesional.ExisteCorreo(correo))
                throw new UsuarioException("Intente con otro correo");
        }
        private void ExisteCI(string CI)
        {
            if(_repositorioCliente.ExisteCI(CI))
                throw new UsuarioException("Ya existe usuario con esa CI");
            if (_repositorioAdmin.ExisteCI(CI))
                throw new UsuarioException("Ya existe usuario con esa CI");
            if (_repositorioProfesional.ExisteCI(CI))
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
                Medias = _mediaServicio.ObtenerImagenesUsuario(Enum_TipoEntidad.Cliente, cliente.Id),
                Telefono = cliente.Telefono,
                Perfil = _mediaServicio.ObtenerImagenPerfil(Enum_TipoEntidad.Cliente, cliente.Id),
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
                Medias = _mediaServicio.ObtenerImagenesUsuario(Enum_TipoEntidad.Admin, admin.Id),
                Telefono = admin.Telefono,
                Perfil = _mediaServicio.ObtenerImagenPerfil(Enum_TipoEntidad.Admin, admin.Id),
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
                Medias = _mediaServicio.ObtenerImagenesUsuario(Enum_TipoEntidad.Profesional, profesional.Id),
                Telefono = profesional.Telefono,
                Perfil = _mediaServicio.ObtenerImagenPerfil(Enum_TipoEntidad.Profesional, profesional.Id),
                UsuarioActivo = profesional.UsuarioActivo
            };
        }

        public void AsignarFotoFavorita(int mediaId, Enum_TipoEntidad tipo, int entidadId)
        {
            _mediaServicio.AsignarFotoFavorita(mediaId, tipo, entidadId);
        }

        public void GuardarCambiosGenerales(UsuarioGenericoDTO dto)
        {
            switch (dto.Rol)
            {
                case "Cliente":
                    Cliente cliente = _repositorioCliente.ObtenerPorId(dto.Id);

                    if (!string.Equals(cliente.Correo, dto.Correo, StringComparison.OrdinalIgnoreCase))
                    {
                        VerificarCorreoUnico(dto.Correo);
                    }

                    cliente.NombreCompleto = dto.Nombre;
                    cliente.Correo = dto.Correo;
                    cliente.Telefono = dto.Telefono;
                    _repositorioCliente.GuardarCambios();
                    break;

                case "Profesional":
                    Profesional profesional = _repositorioProfesional.ObtenerPorId(dto.Id);

                    if (!string.Equals(profesional.Correo, dto.Correo, StringComparison.OrdinalIgnoreCase))
                    {
                        VerificarCorreoUnico(dto.Correo);
                    }

                    profesional.NombreCompleto = dto.Nombre;
                    profesional.Correo = dto.Correo;
                    profesional.Telefono = dto.Telefono;
                    _repositorioProfesional.GuardarCambios();
                    break;

                case "Admin":
                    Admin admin = _repositorioAdmin.ObtenerPorId(dto.Id);

                    if (!string.Equals(admin.Correo, dto.Correo, StringComparison.OrdinalIgnoreCase))
                    {
                        VerificarCorreoUnico(dto.Correo);
                    }

                    admin.NombreCompleto = dto.Nombre;
                    admin.Correo = dto.Correo;
                    admin.Telefono = dto.Telefono;
                    _repositorioAdmin.GuardarCambios();
                    break;

                default:
                    throw new Exception("Rol desconocido.");
            }
        }
        public void CambiarPassword(int id, string rol, string nuevaPass)
        {
            string hash = HashContrasena.Hashear(nuevaPass);

            switch (rol)
            {
                case "Cliente":
                    Cliente cliente = _repositorioCliente.ObtenerPorId(id);
                    cliente.Pass = hash;
                    _repositorioCliente.GuardarCambios();
                    break;

                case "Profesional":
                    Profesional profesional = _repositorioProfesional.ObtenerPorId(id);
                    profesional.Pass = hash;
                    _repositorioProfesional.GuardarCambios();
                    break;

                case "Admin":
                    Admin admin = _repositorioAdmin.ObtenerPorId(id);
                    admin.Pass = hash;
                    _repositorioAdmin.GuardarCambios();
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
                    Admin admin = _repositorioAdmin.ObtenerPorId(usuarioId);
                    if (admin != null && HashContrasena.Verificar(admin.Pass, password))
                    {
                        admin.UsuarioActivo = false;
                        _repositorioAdmin.GuardarCambios();
                    }
                    else
                    {
                        throw new Exception("Verificar contraseña ingresada.");
                    }
                    break;
                case "Profesional":
                    Profesional profesional = _repositorioProfesional.ObtenerPorId(usuarioId);
                    if (profesional != null && HashContrasena.Verificar(profesional.Pass, password))
                    {
                        profesional.UsuarioActivo = false;
                        _repositorioProfesional.GuardarCambios();
                    }
                    else
                    {
                        throw new Exception("Verificar contraseña ingresada.");
                    }
                    break;
                case "Cliente":
                    Cliente cliente = _repositorioCliente.ObtenerPorId(usuarioId);
                    if (cliente != null && HashContrasena.Verificar(cliente.Pass, password))
                    {
                        cliente.UsuarioActivo = false;
                        _repositorioCliente.GuardarCambios();
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
