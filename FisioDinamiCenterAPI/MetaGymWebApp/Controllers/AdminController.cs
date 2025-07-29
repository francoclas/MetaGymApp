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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MetaGymWebApp.Controllers
{
    [AutorizacionRol("Admin")]
    public class AdminController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IExtraServicio _extraServicio;
        private readonly IAdminServicio _adminServicio;
        private readonly IProfesionalServicio _profesionalServicio;
        private readonly IClienteServicio _clienteServicio;
        private readonly IMediaServicio _mediaServicio;
        private readonly IPublicacionServicio  _publicacionServicio;
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
        public IActionResult Index()
        {
            return View();
        }

        //Creacion de usuarios
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View(new CrearUsuarioDTO());
        }
        [HttpPost]
        public IActionResult CrearUsuario(CrearUsuarioDTO dto)
        {
            if (!ModelState.IsValid) {
                
                TempData["Mensaje"] = "Ingrese los correspondientes";
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }

            try
            {
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

                TempData["Mensaje"] = "Usuario creado correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("CrearUsuario");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("CrearUsuario");
            }
        }
        //Menu general
        [HttpGet]
        public IActionResult PanelControl() { 
            PanelControlAdminModel model = new PanelControlAdminModel();
            model.Establecimientos = _extraServicio.ObtenerEstablecimientos();
            model.Especialidades = _extraServicio.ObtenerEspecialidades();  
            return View(model); 
            
        }
        
        //Seccion usuarios
        [HttpGet]
        public IActionResult GestionUsuarios()
        {
            //Obtengo lista
            List<UsuarioGenericoDTO> salida = ObtenerUsuariosListado();
            //Devuelvo
            return View(salida);
        }
        [HttpGet]
        public IActionResult EditarUsuario(int id, string rol)
        {
            //Devuelvo vista de editar segun rol
            return rol switch
            {
                "Cliente" => RedirectToAction("EditarCliente", new { id }),
                "Profesional" => RedirectToAction("EditarProfesional", new { id }),
                "Admin" => RedirectToAction("EditarAdmin", new { id }),
                _ => RedirectToAction("GestionUsuarios")
            };
        }
        //Editar cliente
        [HttpGet]
        public IActionResult EditarCliente(int id)
        {
            try
            {
                //Instancio cliente desde el repo
                Cliente cliente = _clienteServicio.ObtenerPorId(id);
                if (cliente == null) throw new Exception("No se logro obtener el usuario");
                UsuarioGenericoDTO dto = new UsuarioGenericoDTO
                {
                    Id = id,
                    Nombre = cliente.NombreCompleto,
                    Usuario = cliente.NombreUsuario,
                    Correo = cliente.Correo,
                    Telefono = cliente.Telefono,
                    Rol = "Cliente"
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
        [HttpPost]
        public IActionResult GuardarEdicionCliente(UsuarioGenericoDTO dto)
        {
            if(dto.Rol != "Cliente")
            {
                TempData["Mensaje"] = "Vuelva a intentarlo mas tarde";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }
            try
            {
                //Obtengo cliente
                Cliente cliente = _clienteServicio.ObtenerPorId(dto.Id);
                if (cliente == null) throw new Exception("No se logro obtener el usuario");
                //Valido si hay cambios en la info, si hay seteo el valor nevo
                if (!string.Equals(cliente.NombreCompleto, dto.Nombre))
                    cliente.NombreCompleto = dto.Nombre;
                if (!string.Equals(cliente.Correo, dto.Correo))
                    cliente.Correo = dto.Correo;
                if (!string.Equals(cliente.Telefono, dto.Telefono))
                    cliente.Telefono = dto.Telefono;
                if (!string.IsNullOrEmpty(dto.Pass))
                {
                    cliente.Pass = HashContrasena.Hashear(dto.Pass);
                }
                //Actualizo en repo
                _clienteServicio.ActualizarCliente(cliente);
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
        //Editar profesional
        [HttpGet]
        public IActionResult EditarProfesional(int id)
        {
            try
            {
                var profesional = _profesionalServicio.ObtenerProfesional(id);
                if (profesional == null)
                    throw new Exception("Profesional no encontrado.");

                var especialidades = _extraServicio.ObtenerEspecialidades();
                ViewBag.EspecialidadesDisponibles = especialidades;

                var especialidadIds = profesional.Especialidades.Select(e => e.Id).ToList();
                 var tipos = _extraServicio.ObtenerTiposAtencionPorEspecialidades(especialidadIds);
                ViewBag.TiposAtencionDisponibles = tipos;

                UsuarioGenericoDTO dto = new UsuarioGenericoDTO
                {
                    Id = profesional.Id,
                    Usuario = profesional.NombreUsuario,
                    Nombre = profesional.NombreCompleto,
                    Correo = profesional.Correo,
                    Pass = "", // no se muestra el actual
                    Telefono = profesional.Telefono,
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
        [HttpPost]
        public IActionResult GuardarEdicionProfesional(UsuarioGenericoDTO dto, int especialidadId)
        {
            if (dto.Rol != "Profesional")
            {
                TempData["Mensaje"] = "Vuelva a intentarlo más tarde.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }

            Profesional profesional = _profesionalServicio.ObtenerProfesional(dto.Id);

            if (profesional == null)
            {
                TempData["Mensaje"] = "Profesional no encontrado.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }

            if (!string.Equals(profesional.NombreCompleto, dto.Nombre))
                profesional.NombreCompleto = dto.Nombre;
            if (!string.Equals(profesional.Correo, dto.Correo))
                profesional.Correo = dto.Correo;
            if (!string.IsNullOrEmpty(dto.Pass))
                profesional.Pass = HashContrasena.Hashear(dto.Pass);
            if (!string.Equals(profesional.Telefono, dto.Telefono))
                profesional.Telefono = dto.Telefono;

            if (especialidadId != 0)
            {
                var especialidad = _extraServicio.ObtenerEspecialidad(especialidadId);
                _profesionalServicio.AgregarEspecialidad(especialidad, profesional);
            }

            _profesionalServicio.ActualizarProfesional(profesional);

            TempData["Mensaje"] = "Se actualizó el profesional correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("GestionUsuarios");
        }
        [HttpPost]
        public IActionResult GuardarTiposAtencionProfesional(int profesionalId, List<int> tiposSeleccionados)
        {
            if (tiposSeleccionados == null || !tiposSeleccionados.Any())
            {
                TempData["Mensaje"] = "No seleccionaste ningún tipo de atención.";
                TempData["TipoMensaje"] = "warning";
                return RedirectToAction("EditarProfesional", new { id = profesionalId });
            }

            var profesional = _profesionalServicio.ObtenerProfesional(profesionalId);
            foreach (var tipoId in tiposSeleccionados)
            {
                var tipo = _extraServicio.ObtenerTipoAtencion(tipoId);
                _profesionalServicio.AgregarTipoAtencion(tipo, profesional);
            }

            TempData["Mensaje"] = "Tipos de atención asignados correctamente.";
            return RedirectToAction("EditarProfesional", new { id = profesionalId });
        }
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
        //Editar admin
        [HttpGet]
        public IActionResult EditarAdmin(int id)
        {
            try
            {
                //Obtengo instancia del admin desde repo
                Admin admin = _adminServicio.ObtenerPorId(id);
                if (admin == null) throw new Exception("No se obtuvo el usuario");

                //Mapeo a usuario generico
                UsuarioGenericoDTO dto = new UsuarioGenericoDTO
                {
                    Id = id,
                    Nombre = admin.NombreCompleto,
                    Usuario = admin.NombreUsuario,
                    Correo = admin.Correo,
                    Telefono = admin.Telefono,
                    Rol = "Admin"
                };
                //Devuelvo vista
                return View(dto);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("GestionUsuarios");
            }
            
        }
        [HttpPost]
        public IActionResult GuardarEdicionAdmin(UsuarioGenericoDTO dto)
        {
            try
            {
                //Verifico que sea admin
                if (dto.Rol != "Admin")
                {
                    TempData["Mensaje"] = "Vuelva a intentarlo mas tarde";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("GestionUsuarios");
                }
                //Obtengo admin
                Admin admin = _adminServicio.ObtenerPorId(dto.Id);
                //valido la info
                if (!string.Equals(admin.NombreCompleto, dto.Nombre))
                    admin.NombreCompleto = dto.Nombre;
                if (!string.Equals(admin.Correo, dto.Correo))
                    admin.Correo = dto.Correo;
                if (!string.Equals(admin.Telefono, dto.Telefono))
                    admin.Telefono = dto.Telefono;
                if (!string.IsNullOrEmpty(dto.Pass))
                {
                    admin.Pass = HashContrasena.Hashear(dto.Pass);
                }
                //Actualizo
                _adminServicio.ActualizarAdmin(admin);
                TempData["Mensaje"] = "Se actualizo usuario: " + admin.Correo;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("GestionUsuarios");
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = e.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("EditarAdmin",dto.Id);
            }
            

        }
        //Seccion extras
        [HttpGet]
        public IActionResult RegistrarEspecialidad()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarEspecialidad(EspecialidadDTO dto)
        {
            //Valido que haya datos en el modelo
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Datos inválidos.";
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }
            //Guardo en el repo
            _extraServicio.RegistrarEspecialidad(dto);
            TempData["Mensaje"] = "Especialidad registrada correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("PanelControl");
        }
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

            EspecialidadDTO dto = new EspecialidadDTO
            {
                Id = entidad.Id,
                NombreEspecialidad = entidad.NombreEspecialidad,
                DescripcionEspecialidad = entidad.DescripcionEspecialidad
            };

            return View(dto);
        }

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
        [HttpGet]
        public IActionResult RegistrarEstablecimiento()
        {
            return View();
        }

    
        [HttpPost]
        public IActionResult RegistrarEstablecimiento(EstablecimientoDTO dto, IFormFile archivo)
        {

            //Reviso que el modelo estebien
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Datos inválidos.";
                TempData["TipoMensaje"] = "danger";
                return View(dto);
            }

            //Instancio nuevo establecimiento para mandar al sistema
            var nuevo = new Establecimiento
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Longitud = dto.Longitud,
                Latitud = dto.Latitud,
                
            };

            //Envio a la bd
            _extraServicio.RegistrarNuevoEstablecimiento(nuevo); 
            //Verifico si cargaron imagen, si es asi, mando imagen al sistema
            if (archivo != null && archivo.Length > 0)
            {
                _mediaServicio.GuardarArchivo(archivo, Enum_TipoEntidad.Establecimiento, nuevo.Id);
            }

            TempData["Mensaje"] = "Establecimiento registrado correctamente.";
            return RedirectToAction("PanelControl");
        }
        //Ediciones
        [HttpGet]
        public IActionResult EditarEstablecimiento(int id)
        {
            try
            {
                //Instancio desde repo
                Establecimiento entidad = _extraServicio.ObtenerEstablecimiento(id);
                if (entidad == null)
                {
                    TempData["Mensaje"] = "No se encontro establecimiento o no existe.";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction("PanelControl");
                }

                var mediaUrl = entidad.Media?.FirstOrDefault()?.Url ?? "/mediaweb/default/gym_default.jpg";
                //Mapeo 
                EstablecimientoDTO dto = new EstablecimientoDTO
                {
                    Id = entidad.Id,
                    Nombre = entidad.Nombre,
                    Direccion = entidad.Direccion,
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

        [HttpPost]
        public IActionResult EditarEstablecimiento(EstablecimientoDTO dto, IFormFile archivo)
        {
            try
            {
                //Instancio desde l repositorio
                Establecimiento establecimiento = _extraServicio.ObtenerEstablecimiento(dto.Id);
                if (establecimiento == null) return NotFound();
                //Mapeo cambios y guardo
                establecimiento.Nombre = dto.Nombre;
                establecimiento.Direccion = dto.Direccion;
                _extraServicio.GuardarCambios();

                //reviso archivos y actualizo
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

        //Eliminar especialidades
        [HttpPost]
        public IActionResult EliminarEspecialidadProfesional(int profesionalId, int especialidadId)
        {
            var profesional = _profesionalServicio.ObtenerProfesional(profesionalId);
            var especialidad = _extraServicio.ObtenerEspecialidad(especialidadId);

            _profesionalServicio.EliminarEspecialidad(especialidad, profesional);

            return RedirectToAction("EditarUsuario", new { id = profesionalId, rol = "Profesional" });
        }
        //Obtener la lista de usuarios para el menu de control usuarios y poder acceder a editarlos
        private List<UsuarioGenericoDTO> ObtenerUsuariosListado()
        {
            //Se mapea de cada lista a usuario generico
            var clientes = _clienteServicio.ObtenerTodos()
            .Select(c => new UsuarioGenericoDTO { Id = c.Id, Usuario = c.NombreUsuario, Nombre = c.NombreCompleto, Correo = c.Correo, Rol = "Cliente" });

            var profesionales = _profesionalServicio.ObtenerTodos()
                .Select(p => new UsuarioGenericoDTO { Id = p.Id, Usuario = p.NombreUsuario, Nombre = p.NombreCompleto, Correo = p.Correo, Rol = "Profesional" });

            var admins = _adminServicio.ObtenerTodos()
                .Select(a => new UsuarioGenericoDTO { Id = a.Id, Usuario = a.NombreUsuario, Nombre = a.NombreUsuario, Correo = a.Correo, Rol = "Admin" });
            //Se devuelve como lista concatenada
            var modelo = clientes.Concat(profesionales).Concat(admins).ToList();
            return modelo;
        }
        //Publicaciones
        [HttpGet]
        public IActionResult ControlPublicaciones()
        {

            //Obtnego publicaciones con estado= "Pendiente" ya mapeadas al DTO
            List<PublicacionDTO> solicitudes = _publicacionServicio.ObtenerPendientes();

            //Mapeo al modelo de la vista
            ControlPublicacionesModelo modelo = new ControlPublicacionesModelo
            {
                PublicacionesPendientes = solicitudes
            };

            return View(modelo);
        }
        [HttpGet]

        public IActionResult MisPublicaciones()
        {
            int adminId = GestionSesion.ObtenerUsuarioId(HttpContext);

            MisPublicacionesAdminModelo modelo = new MisPublicacionesAdminModelo
            {
                //Obtengo historial del admin y mapeo al modelo
                PublicacionesCreadas = _publicacionServicio.ObtenerCreadasPorAdmin(adminId),
                PublicacionesAutorizadas = _publicacionServicio.ObtenerAutorizadasPorAdmin(adminId),
                PublicacionesRechazadas = _publicacionServicio.ObtenerRechazadasPorAdmin(adminId)
            };

            return View(modelo);
        }

        [HttpGet]
        public IActionResult CrearPublicacion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearPublicacion(CrearPublicacionDTO dto)
        {
            try
            {
                //Verifico los datos ingresados
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
                //Mapeo a nueva publicacion
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
                //Envio a repo
                _publicacionServicio.CrearPublicacionAdmin(publicacion);

                //Recorro si tiene archivos y guardo
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

        [HttpGet]
        public IActionResult DetallesPublicacion(int id)
        {
            //Obtengo publicacion desde repo
            PublicacionDTO publicacion = _publicacionServicio.ObtenerPorId(id);
            //Envio a menu si no existe
            if (publicacion == null)
            {
                TempData["Mensaje"] = "No se encontro la publicacion o no existe.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("ControlPublicaciones");
            }
            ;
            return View(publicacion);
        }
        //Revision de publicacion
        [HttpGet]
        public IActionResult RevisionPublicacion(int id)
        {
            //Instancio publicacion
            var publicacion = _publicacionServicio.ObtenerPorId(id);
            //valido que exista si no lo devuelvo al menu de publicaciones
            if (publicacion == null || publicacion.Estado != Enum_EstadoPublicacion.Pendiente)
            {
                TempData["Mensaje"] = "La publicación no existe o ya fue revisada.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("ControlPublicaciones");
            }

            return View("RevisionPublicacion", publicacion);
        }
      
        [HttpPost]
        public IActionResult ConfirmarRevision(int PublicacionId, string Accion, string? MotivoRechazo)
        {
            try
            {
                //Evaluo si se acepta o se rechaza
                if (Accion == "Aceptar")
                {
                    //Apruebo la publicacion
                    _publicacionServicio.AprobarPublicacion(PublicacionId, GestionSesion.ObtenerUsuarioId(HttpContext));
                    TempData["Mensaje"] = "Publicación aprobada correctamente.";
                }
                else if (Accion == "Rechazar")
                {
                    //Verifico que haya motivo de rechazo
                    if (string.IsNullOrWhiteSpace(MotivoRechazo))
                        throw new Exception("Debe especificar el motivo del rechazo.");
                    //Rechazo la publicacion
                    _publicacionServicio.RechazarPublicacion(PublicacionId, MotivoRechazo, GestionSesion.ObtenerUsuarioId(HttpContext));
                    TempData["Mensaje"] = "Publicación rechazada correctamente.";
                }
                else
                {
                    throw new Exception("Acción no reconocida.");
                }
                //Devuelvo a control
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
        [HttpGet]
        public IActionResult EditarPublicacion(int id)
        {

            //Obtengo publicacion desdde repo
            PublicacionDTO publicacion = _publicacionServicio.ObtenerPorId(id);
            if (publicacion == null) {
                TempData["Mensaje"] = "No se encontro publicacion";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisPublicaciones");
            }

            return View(publicacion);
        }
        [HttpPost]
        public IActionResult EditarPublicacion(int Id, string Titulo, string Descripcion, bool EsPrivada, bool MostrarEnNoticiasPublicas, List<IFormFile> archivos)
        {
            try
            {
                //Instancio publicacion
                PublicacionDTO pub = _publicacionServicio.ObtenerPorId(Id);
                //Valido que exista
                if (pub == null) throw new Exception("Publicación no encontrada.");
                if (string.IsNullOrEmpty(Titulo)) throw new Exception("El titulo no puede estar vacia.");
                //Seteo los cambios
                pub.Titulo = Titulo;
                pub.Descripcion = Descripcion;
                pub.EsPrivada = EsPrivada;
                pub.MostrarEnNoticiasPublicas = MostrarEnNoticiasPublicas;
                //Actuaolizo en el repositori
                _publicacionServicio.ActualizarPublicacion(pub);


                //Si hay archivos en la publicacion(contando los actuales si existen)
                if (archivos != null && archivos.Any())
                {
                    foreach (var archivo in archivos)
                        //Actualizo
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
        [HttpPost]
        public IActionResult EliminarMedia(int mediaId, int publicacionId)
        {
            //Para poder borrar  imagenes
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
