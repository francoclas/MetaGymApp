using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class EstablecimientoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string? UrlMedia { get; set; }
        public Media? Media { get; set; }
        public class EstablecimientoPreviewDTO
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Direccion { get; set; }
            public string? UrlMedia { get; set; }
            public Media? Media { get; set; }
        }
    }
}
