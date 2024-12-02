using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Actividades_Semestral.Models;

public partial class Inscripcione
{
    public int IdInscripcion { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdActividad { get; set; }

    public DateTime? FechaInscripcion { get; set; }

    public int? IdEstado { get; set; }
    [JsonIgnore]
    public virtual Actividade? IdActividadNavigation { get; set; }
    [JsonIgnore]
    public virtual EstadoActividade? IdEstadoNavigation { get; set; }
    [JsonIgnore]
    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
