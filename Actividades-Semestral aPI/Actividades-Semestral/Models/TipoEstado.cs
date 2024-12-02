using System;
using System.Collections.Generic;

namespace Actividades_Semestral.Models;

public partial class TipoEstado
{
    public int IdTipoEstado { get; set; }

    public string NombreTipo { get; set; } = null!;

    public virtual ICollection<EstadoActividade> EstadoActividades { get; set; } = new List<EstadoActividade>();
}
