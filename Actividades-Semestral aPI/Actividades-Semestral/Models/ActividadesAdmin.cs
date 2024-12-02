using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Actividades_Semestral.Models
{
    public class ActividadesAdmin
    {
        public int IdActividad { get; set; }
        public string NombreActividad { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public int CuposMaximos { get; set; }
        public int CuposOcupados { get; set; }
        public DateTime Fecha { get; set; }
    }
}
