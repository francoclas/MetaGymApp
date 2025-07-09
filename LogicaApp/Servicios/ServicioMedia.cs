using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (archivo == null || archivo.Length == 0)
                throw new Exception("No se proporcionó un archivo válido.");

            // Determinar tipo de archivo
            var extension = Path.GetExtension(archivo.FileName).ToLower();
            Enum_TipoMedia tipoMedia;

            if (extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp")
                tipoMedia = Enum_TipoMedia.Imagen;
            else if (extension is ".mp4" or ".avi" or ".mov" or ".webm")
                tipoMedia = Enum_TipoMedia.Video;
            else
                tipoMedia = Enum_TipoMedia.Archivo;

            // Nombre único para el archivo
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var nombreArchivo = $"{tipoEntidad}_{idEntidad}_{timestamp}{extension}";

            // Subcarpeta según tipo de archivo y entidad
            string tipoCarpeta = tipoMedia switch
            {
                Enum_TipoMedia.Imagen => "Imagenes",
                Enum_TipoMedia.Video => "Videos",
                Enum_TipoMedia.Archivo => "Documentos",
                _ => "Otros"
            };

            string subcarpeta = Path.Combine(tipoCarpeta, tipoEntidad.ToString(), idEntidad.ToString());
            var rutaFisicaCarpeta = Path.Combine("wwwroot", "MediaWeb", subcarpeta);
            Directory.CreateDirectory(rutaFisicaCarpeta);

            // Guardar archivo físico
            var rutaCompleta = Path.Combine(rutaFisicaCarpeta, nombreArchivo);
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            // Crear objeto Media
            var media = new Media
            {
                Url = $"/MediaWeb/{subcarpeta.Replace("\\", "/")}/{nombreArchivo}",
                Tipo = tipoMedia,
                TipoEntidad = tipoEntidad
            };

            // Relación con entidad
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

        public Media? ObtenerFotoFavorita(Enum_TipoEntidad tipo, int idEntidad)
        {
            return _repositorio.ObtenerFavorita(tipo, idEntidad);
            
        }

        public void AsignarFotoFavorita(int mediaId, Enum_TipoEntidad tipo, int entidadId)
        {
            _repositorio.AsignarFotoFavorita(mediaId, tipo, entidadId);
        }
        //Usuarios
        public List<Media> ObtenerImagenesUsuario(Enum_TipoEntidad tipo, int idEntidad)
        {
            return _repositorio.ObtenerImagenesUsuario(tipo, idEntidad);
        }

        public Media? ObtenerImagenPerfil(Enum_TipoEntidad tipo, int idEntidad)
        {
            return _repositorio.ObtenerImagenPerfil(tipo, idEntidad);
        }

        public List<Media> ObtenerMediasPorEntidadGeneral(Enum_TipoEntidad tipo, int idEntidad)
        {
            return _repositorio.ObtenerPorEntidadGeneral(tipo, idEntidad);
        }
    }
}
