using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;

namespace APIClienteMetaGym.Extra
{
    public class MapeadorRutinas
    {
        public RutinaDTO MapearRutinaDTO(Rutina rutina)
        {
            RutinaDTO salida = new RutinaDTO();
            salida.Id = rutina.Id;
            salida.NombreRutina = rutina.NombreRutina;
            salida.Tipo = rutina.Tipo;
            salida.FechaCreacion = rutina.FechaCreacion;
            salida.Ejercicios = new List<EjercicioDTO>();
            foreach (var item in rutina.Ejercicios)
            {
                salida.Ejercicios.Add(new EjercicioDTO
                {
                    Id = item.EjercicioId,
                    Nombre = item.Ejercicio.Nombre,
                    Tipo = item.Ejercicio.Tipo,
                    GrupoMuscular = item.Ejercicio.GrupoMuscular,
                    Instrucciones = item.Ejercicio.Instrucciones,
                    ImagenBaseUrl = item.Ejercicio.Medias.FirstOrDefault().Url,
                    MediasURL = DevolverURLS(item.Ejercicio.Medias),
                    MedicionesDTO = DevolverMediciones(item.Ejercicio.Mediciones)
                });
            }
            return salida;
        }

        private List<string> DevolverURLS(List<Media> medias)
        {
            List<string> urls = new List<string>();
            foreach (Media media in medias)
            {
                urls.Add(media.Url);
            }
            return urls;
        }
        private List<MedicionDTO> DevolverMediciones(List<Medicion> mediciones)
        {
            List<MedicionDTO> medis = new List<MedicionDTO>();
            foreach (Medicion item in mediciones)
            {
                medis.Add(new MedicionDTO { MedicionId = item.Id,Nombre = item.Nombre, Unidad = item.Unidad ,Desc = item.Descripcion});
            }

            return medis;
        }
      
    }
}
