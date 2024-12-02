using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Actividades_Semestral.Models;

public partial class EstadoActividade
{
    public int IdEstado { get; set; }

    public string NombreEstado { get; set; } = null!;

    public int? IdTipoEstado { get; set; }
    [JsonIgnore]
    public virtual ICollection<Actividade> Actividades { get; set; } = new List<Actividade>();
    [JsonIgnore]
    public virtual TipoEstado? IdTipoEstadoNavigation { get; set; }
    [JsonIgnore]
    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
    [JsonIgnore]
    public virtual ICollection<PropuestasActividade> PropuestasActividades { get; set; } = new List<PropuestasActividade>();

}
