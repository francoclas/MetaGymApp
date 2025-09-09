using System.Runtime.Intrinsics.X86;
using Humanizer;
using LogicaApp.Servicios;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using MetaGymWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MetaGymWebApp.Controllers
{
    // Controlador para todo lo que maneja el admin en la web
    [AutorizacionRol("Admin")]
    public class AdminController : Controller
    {
        // Servicios que usa el admin para trabajar (usuarios, extras, publicaciones, etc.)
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IExtraServicio _extraServicio;
        private readonly IAdminServicio _adminServicio;
        private readonly IProfesionalServicio _profesionalServicio;
        private readonly IClienteServicio _clienteServicio;
        private readonly IMediaServicio _mediaServicio;
        private readonly IPublicacionServicio _publicacionServicio;

        // Constructor: se inyectan todos los servicios necesarios
        public AdminController(IUsuarioServicio u, IExtraServicio extraServicio, IAdminServicio adminServicio, IProfesionalServicio profesionalServicio, IClienteServicio clienteServicio, IMediaServicio mediaServicio, IPublicacionServicio publicacionServicio)
        {
            _usuarioServicio = u;
            _extraServicio = extraServicio;
            _adminServicio = adminServicio;
            _profesionalServicio = profesionalServicio;
            _clienteServicio = clienteServicio;
            _mediaServicio = mediaServicio;
            _publicacionServicio = publicacionServicio;
        }

        // Página de inicio del panel admin
        public IActionResult Index()
        {
            return View();
        }

        // =======================
        // Creación de usuarios
        // =======================

        // Muestra el form para crear usuario
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View(new CrearUsuarioDTO());
        }

        // Recibe el form y crea el usuario según rol
        [HttpPost]
        public IActionResult CrearUsuario(CrearUsuarioDTO dto)
        {
            // Si el form viene mal, vuelvo con mensaje
            if (!ModelState.IsValid)
            {

                TempData["Mensaje"] = "Ingrese los correspondientes";
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }
            try
            {
                if (!FuncionesAuxiliares.EsCorreoValido(dto.Correo))
                    throw new Exception("Debe ingresar un correo valido.");
                if (!FuncionesAuxiliares.EsTelefonoValido(dto.Telefono))
                    throw new Exception("Debe ingresar un Telefono valido.");
                if (!FuncionesAuxiliares.EsContrasenaValida(dto.Password))
                    throw new Exception("Debe ingresar un Password valido.");
                if(!FuncionesAuxiliares.EsCedulaValida(dto.Ci))
                    throw new Exception("Debe ingresar una Cedula Digital valida.");
                // Valido que no haya usuario/correo repetidos
                _usuarioServicio.VerificarUsuarioRepetido(dto.Usuario, dto.Correo);
                // Según el rol, llamo a la creación que corresponda
                switch (dto.Rol)
                {
                    case "Admin":
                        _usuarioServicio.CrearAdmin(dto.Ci, dto.Usuario, dto.NombreCompleto, dto.Correo, dto.Password, dto.Telefono);
                        break;
                    case "Profesional":
                        _usuarioServicio.CrearProfesional(dto.Ci, dto.Usuario, dto.NombreCompleto, dto.Correo, dto.Password, dto.Telefono);
                        break;
                    case "Cliente":
                        _usuarioServicio.CrearCliente(dto.Ci, dto.Usuario, dto.NombreCompleto, dto.Correo, dto.Password, dto.Telefono);
                        break;
                }

                // Feedback positivo
                TempData["Mensaje"] = "Usuario creado correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("CrearUsuario", dto);
            }
            catch (Exception e)
            {
                // Cualquier error se informa por TempData y se vuelve al form
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View("CrearUsuario", dto);
            }
        }

        // =======================
        // Menú general (dashboard)
        // =======================

        // Carga datos básicos para el panel (listas para mostrar)
        [HttpGet]
        public IActionResult PanelControl()
        {
            PanelControlAdminModel model = new PanelControlAdminModel();
            model.Establecimientos = _extraServicio.ObtenerEstablecimientos();
            model.Especialidades = _extraServicio.ObtenerEspecialidades();
            return View(model);

        }

        // =======================
        // Gestión de usuarios
        // =======================

        // Pantalla que lista todos los usuarios (cliente/profesional/admin)
        [HttpGet]
        public IActionResult GestionUsuarios()
        {
            // Armo la lista combinada
            List<UsuarioGenericoDTO> salida = ObtenerUsuariosListado();
            // Devuelvo la vista con el modelo
            return View(salida);
        }

        // Redirecciona a la vista de edición según el rol elegido
        [HttpGet]
        public IActionResult EditarUsuario(int id, string rol)
        {
            // Redirijo a la acción concreta de edición
            return rol switch
            {
                "Cliente" => RedirectToAction("EditarCliente", new { id }),
                "Profesional" => RedirectToAction("EditarProfesional", new { id }),
                "Admin" => RedirectToAction("EditarAdmin", new { id }),
                _ => RedirectToAction("GestionUsuarios")
            };
        }

        // ---------- Clientes ----------

        // Carga datos de un cliente para editar
        [HttpGet]
        public IActionResult EditarCliente(int id)
        {
            try
            {
                // Busco cliente
                Cliente cliente = _clienteServicio.ObtenerPorId(id);
                if (cliente == null) throw new Exception("No se logro obtener el usuario");
                // Mapeo a DTO genérico para la vista
                UsuarioGenericoDTO dto = new UsuarioGenericoDTO
                {
                    Id = id,
                    Nombre = cliente.NombreCompleto,
                    Usuario = cliente.NombreUsuario,
                    Correo = cliente.Correo,
                    Telefono = cliente.Telefono,
                    UsuarioActivo = cliente.UsuarioActivo,
                    Rol = "Cliente"
                };
                return View(dto);
            }
            catch (Exception e)
            {
                // Si falla, vuelvo a la lista con mensaje
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }

        }

        // Guarda cambios de un cliente
        [HttpPost]
        public IActionResult GuardarEdicionCliente(UsuarioGenericoDTO dto)
        {
            // Seguridad básica: que no venga con rol cambiado
            if (dto.Rol != "Cliente")
            {
                TempData["Mensaje"] = "Vuelva a intentarlo mas tarde";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }
            try
            {
                // Obtengo el cliente que se quiere editar
                Cliente cliente = _clienteServicio.ObtenerPorId(dto.Id);
                if (cliente == null) throw new Exception("No se logro obtener el usuario");
                // Actualizo solo si cambió algo, así evito escrituras innecesarias
                if (!string.Equals(cliente.NombreCompleto, dto.Nombre))
                    if (string.IsNullOrEmpty(dto.Nombre))
                        throw new Exception("El nombre no puedo ser vacio");
                    cliente.NombreCompleto = dto.Nombre;
                if (!string.Equals(cliente.Correo, dto.Correo))
                {
                    if (string.IsNullOrEmpty(dto.Correo))
                        throw new Exception("El correo nuevo no puede ser vacio");
                    if (!FuncionesAuxiliares.EsCorreoValido(dto.Correo))
                        throw new Exception("El correo no es valido.");
                    // Si cambia el correo, me aseguro que no esté repetido
                    _usuarioServicio.VerificarCorreoUnico(dto.Correo);
                    cliente.Correo = dto.Correo;
                }
                if (!string.Equals(cliente.Telefono, dto.Telefono))
                {
                    if (string.IsNullOrEmpty(dto.Telefono))
                        throw new Exception("El telefono no puede ser vacio.");
                    cliente.Telefono = dto.Telefono;
                }
                // Cambio de contraseña (si se mandó algo)
                if (!string.IsNullOrEmpty(dto.Pass))
                {
                    if (!FuncionesAuxiliares.EsContrasenaValida(dto.Pass))
                        throw new Exception("La contraseña ingresada no es valida");
                    cliente.Pass = HashContrasena.Hashear(dto.Pass);
                }
                // Activo/inactivo
                cliente.UsuarioActivo = dto.UsuarioActivo;
                // Persisto
                _clienteServicio.ActualizarCliente(cliente);
                // Feedback
                TempData["Mensaje"] = "Se actualizo usuario: " + cliente.Correo;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionUsuarios");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }

        }

        // ---------- Profesionales ----------

        // Carga datos de un profesional para editar (incluye especialidades y tipos de atención)
        [HttpGet]
        public IActionResult EditarProfesional(int id)
        {
            try
            {
                var profesional = _profesionalServicio.ObtenerProfesional(id);
                if (profesional == null)
                    throw new Exception("Profesional no encontrado.");

                // Listas para selects en la vista
                var especialidades = _extraServicio.ObtenerEspecialidades();
                ViewBag.EspecialidadesDisponibles = especialidades;

                var especialidadIds = profesional.Especialidades.Select(e => e.Id).ToList();
                var tipos = _extraServicio.ObtenerTiposAtencionPorEspecialidades(especialidadIds);
                ViewBag.TiposAtencionDisponibles = tipos;

                // Mapeo a DTO genérico para edición
                UsuarioGenericoDTO dto = new UsuarioGenericoDTO
                {
                    Id = profesional.Id,
                    Usuario = profesional.NombreUsuario,
                    Nombre = profesional.NombreCompleto,
                    Correo = profesional.Correo,
                    Pass = "", // no se muestra el actual
                    Telefono = profesional.Telefono,
                    UsuarioActivo = profesional.UsuarioActivo,
                    Especialidades = profesional.Especialidades,
                    TiposAtencionIds = profesional.TiposAtencion?.Select(t => t.Id).ToList() ?? new List<int>(),
                    Rol = "Profesional",
                    TiposAtencionAsignadas = profesional.TiposAtencion
                };

                return View(dto);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }
        }

        // Guarda cambios de un profesional (datos base + alta de especialidad puntual)
        [HttpPost]
        public IActionResult GuardarEdicionProfesional(UsuarioGenericoDTO dto, int especialidadId)
        {

            try
            {
                // Seguridad: que sea profesional
                if (dto.Rol != "Profesional")
                {
                    TempData["Mensaje"] = "Vuelva a intentarlo más tarde.";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("GestionUsuarios");
                }

                // Busco el profesional
                Profesional profesional = _profesionalServicio.ObtenerProfesional(dto.Id);

                if (profesional == null)
                {
                    TempData["Mensaje"] = "Profesional no encontrado.";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("GestionUsuarios");
                }

                // Actualizo campos si cambiaron
                if (!string.Equals(profesional.NombreCompleto, dto.Nombre))
                    profesional.NombreCompleto = dto.Nombre;
                if (!string.Equals(profesional.Correo, dto.Correo))
                {
                    if(string.IsNullOrEmpty(dto.Correo))
                        throw new Exception("El correo nuevo no puede ser vacio");
                    if (!FuncionesAuxiliares.EsCorreoValido(dto.Correo))
                        throw new Exception("Debe ingresar un correo valido");
                    _usuarioServicio.VerificarCorreoUnico(dto.Correo);
                    profesional.Correo = dto.Correo;
                }
                if (!string.IsNullOrEmpty(dto.Pass))
                    profesional.Pass = HashContrasena.Hashear(dto.Pass);
                if (!string.Equals(profesional.Telefono, dto.Telefono)) { 
                    if (string.IsNullOrEmpty(dto.Telefono))
                        throw new Exception("El telefono nuevo no puede ser vacio");
                    if (!FuncionesAuxiliares.EsTelefonoValido(dto.Telefono))
                            throw new Exception("Debe ingresar un telefono valido");
                    profesional.Telefono = dto.Telefono;
                }
                profesional.UsuarioActivo = dto.UsuarioActivo;

                // Alta rápida de una especialidad (si vino una)
                if (especialidadId != 0)
                {
                    var especialidad = _extraServicio.ObtenerEspecialidad(especialidadId);
                    _profesionalServicio.AgregarEspecialidad(especialidad, profesional);
                }

                // Persisto cambios
                _profesionalServicio.ActualizarProfesional(profesional);

                TempData["Mensaje"] = "Se actualizó el profesional correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("EditarProfesional", new { dto.Id });
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarProfesional", new { dto.Id });
            }

        }

        // Asigna tipos de atención al profesional
        [HttpPost]
        public IActionResult GuardarTiposAtencionProfesional(int profesionalId, List<int> tiposSeleccionados)
        {
            try
            {
                // Debe venir algo seleccionado
                if (tiposSeleccionados == null || !tiposSeleccionados.Any())
                {
                    TempData["Mensaje"] = "No seleccionaste ningún tipo de atención.";
                    TempData["TipoMensaje"] = "warning";
                    return RedirectToAction("EditarProfesional", new { id = profesionalId });
                }

                var profesional = _profesionalServicio.ObtenerProfesional(profesionalId);
                // Asigno cada tipo al profesional
                foreach (var tipoId in tiposSeleccionados)
                {
                    var tipo = _extraServicio.ObtenerTipoAtencion(tipoId);
                    _profesionalServicio.AgregarTipoAtencion(tipo, profesional);
                }

                TempData["Mensaje"] = "Tipos de atención asignados correctamente.";
                return RedirectToAction("EditarProfesional", new { id = profesionalId });
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                return RedirectToAction("EditarProfesional", new { id = profesionalId });
            }

        }

        // Quita un tipo de atención del profesional
        [HttpPost]
        public IActionResult EliminarTipoAtencionProfesional(int profesionalId, int tipoAtencionId)
        {
            try
            {
                _profesionalServicio.EliminarTipoAtencion(profesionalId, tipoAtencionId);
                TempData["Mensaje"] = "Tipo de atención eliminado.";
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction("EditarProfesional", new { id = profesionalId });
        }

        // ---------- Admins ----------

        // Carga datos de un admin para editar
        [HttpGet]
        public IActionResult EditarAdmin(int id)
        {
            try
            {
                // Busco instancia del admin
                Admin admin = _adminServicio.ObtenerPorId(id);
                if (admin == null) throw new Exception("No se obtuvo el usuario");

                // Mapeo a DTO genérico
                UsuarioGenericoDTO dto = new UsuarioGenericoDTO
                {
                    Id = id,
                    Nombre = admin.NombreCompleto,
                    Usuario = admin.NombreUsuario,
                    Correo = admin.Correo,
                    Telefono = admin.Telefono,
                    UsuarioActivo = admin.UsuarioActivo,
                    Rol = "Admin"

                };
                // Devuelvo la vista
                return View(dto);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }

        }

        // Guarda cambios de admin
        [HttpPost]
        public IActionResult GuardarEdicionAdmin(UsuarioGenericoDTO dto)
        {
            try
            {
                // Seguridad: que efectivamente sea admin
                if (dto.Rol != "Admin")
                {
                    TempData["Mensaje"] = "Vuelva a intentarlo mas tarde";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("GestionUsuarios");
                }
                // Busco admin
                Admin admin = _adminServicio.ObtenerPorId(dto.Id);
                // Actualizo campos si cambiaron
                if (!string.Equals(admin.NombreCompleto, dto.Nombre))
                    admin.NombreCompleto = dto.Nombre;
                if (!string.Equals(admin.Correo, dto.Correo))
                {
                    if (!FuncionesAuxiliares.EsCorreoValido(dto.Correo))
                        throw new Exception("El correo no es valido");
                    _usuarioServicio.VerificarCorreoUnico(dto.Correo);
                    admin.Correo = dto.Correo;
                }
                if (!string.Equals(admin.Telefono, dto.Telefono))
                    if(!FuncionesAuxiliares.EsTelefonoValido(dto.Telefono))
                        throw new Exception("El telefono no es valido");
                admin.Telefono = dto.Telefono;
                if (!string.IsNullOrEmpty(dto.Pass))
                {
                    if (!FuncionesAuxiliares.EsContrasenaValida(dto.Pass))
                        throw new Exception("La contraseña no es segura");
                    admin.Pass = HashContrasena.Hashear(dto.Pass);
                }
                admin.UsuarioActivo = dto.UsuarioActivo;
                // Persisto cambios
                _adminServicio.ActualizarAdmin(admin);
                TempData["Mensaje"] = "Se actualizo usuario: " + admin.Correo;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionUsuarios");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarAdmin", new { dto.Id });
            }


        }

        // =======================
        // Sección: Extras
        // =======================

        // Vista para registrar una especialidad
        [HttpGet]
        public IActionResult RegistrarEspecialidad()
        {
            return View();
        }

        // Alta de especialidad
        [HttpPost]
        public IActionResult RegistrarEspecialidad(EspecialidadDTO dto)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrEmpty(dto.NombreEspecialidad)) throw new Exception("Debe ingresar un nombre.");
                if (string.IsNullOrEmpty(dto.DescripcionEspecialidad)) throw new Exception("Debe ingresar una descripcion.");

                // Guardo en repo
                _extraServicio.RegistrarEspecialidad(dto);
                TempData["Mensaje"] = "Especialidad registrada correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("PanelControl");
            }
            catch (Exception e)
            {
                // OJO: acá se estaba guardando e (objeto) en TempData["Mensaje"].
                // Mantengo tu lógica exacta, pero lo dejo comentado para revisar en el futuro.
                TempData["Mensaje"] = e;
                TempData["TipoMensaje"] = "danger";
                return View("RegistrarEspecialidad");
            }

        }

        // Cargar una especialidad para editar
        [HttpGet]
        public IActionResult EditarEspecialidad(int id)
        {
            var entidad = _extraServicio.ObtenerEspecialidad(id);
            if (entidad == null)
            {
                TempData["Mensaje"] = "No se encontró especialidad.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("PanelControl");
            }

            // Mapeo a DTO
            EspecialidadDTO dto = new EspecialidadDTO
            {
                Id = entidad.Id,
                NombreEspecialidad = entidad.NombreEspecialidad,
                DescripcionEspecialidad = entidad.DescripcionEspecialidad
            };

            return View(dto);
        }

        // Guardar edición de especialidad
        [HttpPost]
        public IActionResult EditarEspecialidad(EspecialidadDTO dto)
        {
            try
            {
                var entidad = _extraServicio.ObtenerEspecialidad(dto.Id);
                if (entidad == null) throw new Exception("No se encontró especialidad.");

                entidad.NombreEspecialidad = dto.NombreEspecialidad;
                entidad.DescripcionEspecialidad = dto.DescripcionEspecialidad;

                _extraServicio.GuardarCambios();

                TempData["Mensaje"] = "Especialidad actualizada.";
                return RedirectToAction("PanelControl");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("PanelControl");
            }
        }

        // Agregar un tipo de atención a una especialidad
        [HttpPost]
        public IActionResult AgregarTipoAtencion(int especialidadId, string nombre, string descripcion)
        {
            var especialidad = _extraServicio.ObtenerEspecialidad(especialidadId);
            if (especialidad == null)
            {
                TempData["Mensaje"] = "No se encontró la especialidad.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarEspecialidad", new { id = especialidadId });
            }

            // Armo el tipo y lo creo
            TipoAtencion nuevo = new TipoAtencion
            {
                Nombre = nombre,
                Descripcion = descripcion,
                EspecialidadId = especialidad.Id
            };

            _extraServicio.CrearTipoAtencion(nuevo);
            TempData["Mensaje"] = "Tipo de atención agregado.";
            return RedirectToAction("EditarEspecialidad", new { id = especialidadId });
        }

        // =======================
        // Sección: Establecimientos
        // =======================

        // Vista para registrar establecimiento
        [HttpGet]
        public IActionResult RegistrarEstablecimiento()
        {
            return View();
        }

        // Alta de establecimiento (+ opcional subir imagen)
        [HttpPost]
        public IActionResult RegistrarEstablecimiento(EstablecimientoDTO dto, IFormFile archivo)
        {

            // Valido el modelo
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Datos inválidos.";
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }

            // Instancia para persistir
            var nuevo = new Establecimiento
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Longitud = dto.Longitud,
                Latitud = dto.Latitud,

            };

            // Guardo establecimiento
            _extraServicio.RegistrarNuevoEstablecimiento(nuevo);
            // Si vino archivo, lo guardo asociado
            if (archivo != null && archivo.Length > 0)
            {
                _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Establecimiento, nuevo.Id);
            }

            TempData["Mensaje"] = "Establecimiento registrado correctamente.";
            return RedirectToAction("PanelControl");
        }

        // Cargar establecimiento para editar
        [HttpGet]
        public IActionResult EditarEstablecimiento(int id)
        {
            try
            {
                // Busco por id
                Establecimiento entidad = _extraServicio.ObtenerEstablecimiento(id);
                if (entidad == null)
                {
                    TempData["Mensaje"] = "No se encontro establecimiento o no existe.";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("PanelControl");
                }

                // Foto por defecto si no hay media asociada
                var mediaUrl = entidad.Media?.FirstOrDefault()?.Url ?? "/mediaweb/default/gym_default.jpg";
                // Mapeo a DTO para la vista
                EstablecimientoDTO dto = new EstablecimientoDTO
                {
                    Id = entidad.Id,
                    Nombre = entidad.Nombre,
                    Direccion = entidad.Direccion,
                    Latitud = entidad.Latitud,
                    Longitud = entidad.Longitud,
                    UrlMedia = mediaUrl
                };
                return View(dto);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("PanelControl");
            }


        }

        // Guardar edición de establecimiento (+ reemplazo de imagen si vino)
        [HttpPost]
        public IActionResult EditarEstablecimiento(EstablecimientoDTO dto, IFormFile archivo)
        {
            try
            {
                // Busco el que se quiere editar
                Establecimiento establecimiento = _extraServicio.ObtenerEstablecimiento(dto.Id);
                if (establecimiento == null) return NotFound();
                // Actualizo campos
                establecimiento.Nombre = dto.Nombre;
                establecimiento.Direccion = dto.Direccion;
                establecimiento.Longitud = dto.Longitud;
                establecimiento.Latitud = dto.Latitud;
                _extraServicio.GuardarCambios();

                // Si vino archivo, reemplazo el existente
                if (archivo != null && archivo.Length > 0)
                {
                    _mediaServicio.ReemplazarArchivo(archivo, Enum_TipoEntidad.Establecimiento, establecimiento.Id);
                }

                TempData["Mensaje"] = "Establecimiento actualizado.";
                return RedirectToAction("PanelControl");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("PanelControl");
            }

        }

        // Quitar una especialidad de un profesional (atajo desde admin)
        [HttpPost]
        public IActionResult EliminarEspecialidadProfesional(int profesionalId, int especialidadId)
        {
            var profesional = _profesionalServicio.ObtenerProfesional(profesionalId);
            var especialidad = _extraServicio.ObtenerEspecialidad(especialidadId);

            _profesionalServicio.EliminarEspecialidad(especialidad, profesional);

            return RedirectToAction("EditarUsuario", new { id = profesionalId, rol = "Profesional" });
        }

        // Helper: arma lista unificada de usuarios para la vista de gestión
        private List<UsuarioGenericoDTO> ObtenerUsuariosListado()
        {
            // Mapeo de cada tipo a un DTO genérico
            var clientes = _clienteServicio.ObtenerTodos()
            .Select(c => new UsuarioGenericoDTO { Id = c.Id, Usuario = c.NombreUsuario, Nombre = c.NombreCompleto, Correo = c.Correo, Rol = "Cliente" });

            var profesionales = _profesionalServicio.ObtenerTodos()
                .Select(p => new UsuarioGenericoDTO { Id = p.Id, Usuario = p.NombreUsuario, Nombre = p.NombreCompleto, Correo = p.Correo, Rol = "Profesional" });

            var admins = _adminServicio.ObtenerTodos()
                .Select(a => new UsuarioGenericoDTO { Id = a.Id, Usuario = a.NombreUsuario, Nombre = a.NombreUsuario, Correo = a.Correo, Rol = "Admin" });
            // Devuelvo concatenado
            var modelo = clientes.Concat(profesionales).Concat(admins).ToList();
            return modelo;
        }

        // =======================
        // Sección: Publicaciones
        // =======================

        // Vista principal para revisar solicitudes pendientes
        [HttpGet]
        public IActionResult ControlPublicaciones()
        {

            // Pido al servicio las publicaciones en estado "Pendiente"
            List<PublicacionDTO> solicitudes = _publicacionServicio.ObtenerPendientes();

            // Armo el modelo de la vista
            ControlPublicacionesModelo modelo = new ControlPublicacionesModelo
            {
                PublicacionesPendientes = solicitudes
            };

            return View(modelo);
        }

        // Historial propio del admin (lo que creó, autorizó o rechazó)
        [HttpGet]

        public IActionResult MisPublicaciones()
        {
            int adminId = GestionSesion.ObtenerUsuarioId(HttpContext);

            MisPublicacionesAdminModelo modelo = new MisPublicacionesAdminModelo
            {
                // Cargo listas desde el servicio
                PublicacionesCreadas = _publicacionServicio.ObtenerCreadasPorAdmin(adminId),
                PublicacionesAutorizadas = _publicacionServicio.ObtenerAutorizadasPorAdmin(adminId),
                PublicacionesRechazadas = _publicacionServicio.ObtenerRechazadasPorAdmin(adminId)
            };

            return View(modelo);
        }

        // Form para crear una publicación como admin
        [HttpGet]
        public IActionResult CrearPublicacion()
        {
            return View();
        }

        // Procesa el alta de publicación del admin (con media opcional)
        [HttpPost]
        public IActionResult CrearPublicacion(CrearPublicacionDTO dto)
        {
            try
            {
                // Validaciones básicas de contenido
                if (string.IsNullOrWhiteSpace(dto.Titulo))
                    throw new Exception("El título es obligatorio.");

                if (string.IsNullOrWhiteSpace(dto.Descripcion))
                    throw new Exception("La descripción es obligatoria.");

                if (dto.Titulo.Length > 100)
                    throw new Exception("El título no puede superar los 100 caracteres.");

                if (dto.Descripcion.Length > 1000)
                    throw new Exception("La descripción no puede superar los 1000 caracteres.");
                var archivos = dto.ArchivosMedia;
                int adminId = GestionSesion.ObtenerUsuarioId(HttpContext);
                // Armo la publicación
                Publicacion publicacion = new Publicacion
                {
                    Titulo = dto.Titulo,
                    Descripcion = dto.Descripcion,
                    FechaCreacion = DateTime.Now,
                    FechaProgramada = dto.FechaProgramada,
                    EsPrivada = dto.EsPrivada,
                    MostrarEnNoticiasPublicas = dto.MostrarEnNoticiasPublicas,
                    AdminCreadorId = adminId,
                    Estado = Enum_EstadoPublicacion.Aprobada,
                    ListaMedia = new List<Media>(),

                };
                // Persisto
                _publicacionServicio.CrearPublicacionAdmin(publicacion);

                // Si vinieron archivos, los guardo asociados
                if (archivos != null && archivos.Any())
                {
                    foreach (var archivo in archivos)
                    {
                        if (archivo.Length > 0)
                        {
                            _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Publicacion, publicacion.Id);
                        }
                    }
                }
                TempData["Mensaje"] = "Se registro la nueva publicacion correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("MisPublicaciones");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return View(dto);

            }

        }

        // Detalles de una publicación específica
        [HttpGet]
        public IActionResult DetallesPublicacion(int id)
        {
            // Busco la publicación
            PublicacionDTO publicacion = _publicacionServicio.ObtenerPorId(id);
            // Si no existe, vuelvo
            if (publicacion == null)
            {
                TempData["Mensaje"] = "No se encontro la publicacion o no existe.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("ControlPublicaciones");
            }
            ;
            return View(publicacion);
        }

        // Vista de revisión (cuando está Pendiente)
        [HttpGet]
        public IActionResult RevisionPublicacion(int id)
        {
            // Busco publicación
            var publicacion = _publicacionServicio.ObtenerPorId(id);
            // Si no existe o ya se revisó, vuelvo
            if (publicacion == null || publicacion.Estado != Enum_EstadoPublicacion.Pendiente)
            {
                TempData["Mensaje"] = "La publicación no existe o ya fue revisada.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("ControlPublicaciones");
            }

            return View("RevisionPublicacion", publicacion);
        }

        // Acción que confirma la revisión (aprobar / rechazar)
        [HttpPost]
        public IActionResult ConfirmarRevision(int PublicacionId, string Accion, string? MotivoRechazo)
        {
            try
            {
                // Si viene aceptar, apruebo
                if (Accion == "Aceptar")
                {
                    _publicacionServicio.AprobarPublicacion(PublicacionId, GestionSesion.ObtenerUsuarioId(HttpContext));
                    TempData["Mensaje"] = "Publicación aprobada correctamente.";
                }
                // Si viene rechazar, valido motivo y rechazo
                else if (Accion == "Rechazar")
                {
                    if (string.IsNullOrWhiteSpace(MotivoRechazo))
                        throw new Exception("Debe especificar el motivo del rechazo.");
                    _publicacionServicio.RechazarPublicacion(PublicacionId, MotivoRechazo, GestionSesion.ObtenerUsuarioId(HttpContext));
                    TempData["Mensaje"] = "Publicación rechazada correctamente.";
                }
                else
                {
                    throw new Exception("Acción no reconocida.");
                }
                // Feedback y vuelvo al listado
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("ControlPublicaciones");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("RevisionPublicacion", new { id = PublicacionId });
            }
        }

        // Carga de publicación para edición manual
        [HttpGet]
        public IActionResult EditarPublicacion(int id)
        {
            PublicacionDTO publicacion = _publicacionServicio.ObtenerPorId(id);
            if (publicacion == null)
            {
                TempData["Mensaje"] = "La publicación no existe.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("ControlPublicaciones");
            }

            // Cargo enum en ViewBag para el select de estado
            ViewBag.Estados = Enum.GetValues(typeof(Enum_EstadoPublicacion))
                                  .Cast<Enum_EstadoPublicacion>()
                                  .Select(e => new SelectListItem
                                  {
                                      Value = ((int)e).ToString(),
                                      Text = e.ToString()
                                  }).ToList();

            return View(publicacion);
        }

        // Guarda cambios manuales de una publicación + agrega media nueva si llega
        [HttpPost]
        public IActionResult EditarPublicacion(int Id, string Titulo, string Descripcion, bool EsPrivada, int Estado, bool MostrarEnNoticiasPublicas, List<IFormFile> archivos)
        {
            try
            {
                // Busco la publicación
                PublicacionDTO pub = _publicacionServicio.ObtenerPorId(Id);
                // Valido existencia y datos mínimos
                if (pub == null) throw new Exception("Publicación no encontrada.");
                if (string.IsNullOrEmpty(Titulo)) throw new Exception("El titulo no puede estar vacia.");
                // Seteo cambios
                pub.Titulo = Titulo;
                pub.Descripcion = Descripcion;
                pub.EsPrivada = EsPrivada;
                pub.MostrarEnNoticiasPublicas = MostrarEnNoticiasPublicas;
                pub.Estado = (Enum_EstadoPublicacion)Estado;
                // Actualizo en el servicio
                _publicacionServicio.ActualizarPublicacion(pub);


                // Si llegan archivos, los guardo asociados
                if (archivos != null && archivos.Any())
                {
                    foreach (var archivo in archivos)
                        _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Publicacion, pub.Id);
                }

                TempData["Mensaje"] = "Publicación actualizada correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("MisPublicaciones");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisPublicaciones");
            }
        }

        // Elimina una media puntual de una publicación
        [HttpPost]
        public IActionResult EliminarMedia(int mediaId, int publicacionId)
        {
            // Para poder borrar imagenes
            try
            {
                _mediaServicio.EliminarMedia(mediaId);
                return RedirectToAction("EditarPublicacion", new { id = publicacionId });
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisPublicaciones");
            }

        }
    }
}