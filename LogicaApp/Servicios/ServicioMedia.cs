using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Http;

namespace LogicaApp.Servicios
{
    public class ServicioMedia : IMediaServicio
    {
        private readonly IRepositorioMedia _repositorio;

        public ServicioMedia(IRepositorioMedia repositorio)
        {
            _repositorio = repositorio;
        }
        public Media GuardarArchivo(IFormFile archivo, Enum_TipoEntidad tipoEntidad, int idEntidad)
        {
            //Valido la informacion recibida
            if (archivo == null || archivo.Length == 0)
                throw new Exception("No se proporcionó un archivo válido.");

            //genero nombre de archivo
            var extension = Path.GetExtension(archivo.FileName);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var nombreArchivo = $"{tipoEntidad}_{idEntidad}_{timestamp}{extension}";

            // Genero ruta de carpeta segun entidad Subcarpeta según tipo y ID
            string subcarpeta = tipoEntidad switch
            {
                //todos dentro de Multimedia
                Enum_TipoEntidad.Cliente => Path.Combine("Usuarios", "cliente", idEntidad.ToString()), //Manda a /Usuarios/cliente/Idcliente
                Enum_TipoEntidad.Profesional => Path.Combine("Usuarios", "profesional", idEntidad.ToString()), //Manda a /Usuarios/profesional/Idprofesional
                Enum_TipoEntidad.Admin => Path.Combine("Usuarios", "admin", idEntidad.ToString()), //Manda a /Usuarios/admin/Idadmin
                Enum_TipoEntidad.Ejercicio => Path.Combine("Ejercicios", idEntidad.ToString()), //Manda a /Ejercicios/IdEJercicio
                Enum_TipoEntidad.Publicacion => Path.Combine("Publicaciones", idEntidad.ToString()), //Manda a /Publicaciones/IdPublicacion
                Enum_TipoEntidad.Establecimiento => Path.Combine("Establecimientos", idEntidad.ToString()), //Manda a /Establecimientos/IdEstablecimiento
                _ => "Otros"//Por posibles implementaciones futuras
            };

            //Finalizo ruta
            var rutaFisicaCarpeta = Path.Combine("wwwroot", "MediaWeb", "Imagenes", subcarpeta);
            Directory.CreateDirectory(rutaFisicaCarpeta);

            //Termino de cargar archivo a su carpeta
            var rutaCompleta = Path.Combine(rutaFisicaCarpeta, nombreArchivo);
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            //Instancio media a cargar
            var media = new Media
            {
                Url = $"/MediaWeb/Imagenes/{subcarpeta.Replace("\\", "/")}/{nombreArchivo}",
                Tipo = Enum_TipoMedia.Imagen
            };

            //Genera la relacion segun el tipo a cargar
            switch (tipoEntidad)
            {
                case Enum_TipoEntidad.Cliente:
                    media.ClienteId = idEntidad;
                    break;
                case Enum_TipoEntidad.Profesional:
                    media.ProfesionalId = idEntidad;
                    break;
                case Enum_TipoEntidad.Admin:
                    media.AdminId = idEntidad;
                    break;
                case Enum_TipoEntidad.Ejercicio:
                    media.EjercicioId = idEntidad;
                    break;
                case Enum_TipoEntidad.Publicacion:
                    media.PublicacionId = idEntidad;
                    break;
                case Enum_TipoEntidad.Establecimiento:
                    media.EstablecimientoId = idEntidad;
                    break;
            }

            //Mando a la bd
            _repositorio.Agregar(media);
            return media;
        }
        public void AsignarMediaPorDefecto(Enum_TipoEntidad tipo, int idEntidad)
        {
            string rutaDefecto = tipo switch
            {
                Enum_TipoEntidad.Admin => "/Media/Default/perfil_default.jpg",
                Enum_TipoEntidad.Ejercicio => "/Media/Default/ejercicio_default.jpg",
                Enum_TipoEntidad.Cliente => "/Media/Default/perfil_default.jpg",
                Enum_TipoEntidad.Profesional => "/Media/Default/perfil_default.jpg",
                Enum_TipoEntidad.Establecimiento => "/Media/Default/gym_default.jpg",
                _ => "/Media/Default/placeholder.jpg"
            };

            var media = new Media
            {
                Url = rutaDefecto,
                Tipo = Enum_TipoMedia.Imagen,
                EstablecimientoId = tipo == Enum_TipoEntidad.Establecimiento ? idEntidad : null,
                EjercicioId = tipo == Enum_TipoEntidad.Ejercicio ? idEntidad : null,
                ClienteId = tipo == Enum_TipoEntidad.Cliente ? idEntidad : null,
                ProfesionalId = tipo == Enum_TipoEntidad.Profesional ? idEntidad : null
            };

            _repositorio.Agregar(media);
            _repositorio.Guardar();
        }
        public Media ObtenerMediaPorEntidad(Enum_TipoEntidad tipoEntidad, int idEntidad)
        {
            return _repositorio.ObtenerPorEntidad(tipoEntidad, idEntidad);
        }
        public void ReemplazarArchivo(IFormFile archivoNuevo, Enum_TipoEntidad tipoEntidad, int idEntidad)
        {
            if (archivoNuevo == null || archivoNuevo.Length == 0)
                throw new Exception("No se proporcionó un archivo válido.");

            // Busco la que hay que modificar
            var mediaExistente = _repositorio.ObtenerPorEntidad(tipoEntidad, idEntidad);
            if (mediaExistente != null)
            {
                //Verifico que se enceuntre la ruta
                var rutaExistente = Path.Combine("wwwroot", mediaExistente.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                //Si existe se elimina
                if (File.Exists(rutaExistente))
                {
                    File.Delete(rutaExistente);
                }
                //Se confirma el eliminar desde la BD
                _repositorio.Eliminar(mediaExistente);
            }

            // Mando la nueva modificada
            GuardarArchivo(archivoNuevo, tipoEntidad, idEntidad);
        }
        public void EliminarMedia(int mediaId)
        {
            var media = _repositorio.ObtenerPorId(mediaId);
            if (media != null)
            {
                var rutaFisica = Path.Combine("wwwroot", media.Url.TrimStart('/'));
                if (File.Exists(rutaFisica)) File.Delete(rutaFisica);
                _repositorio.Eliminar(media);
            }
        }
    }
}
