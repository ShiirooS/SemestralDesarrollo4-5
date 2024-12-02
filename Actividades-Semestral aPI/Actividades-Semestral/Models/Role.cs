using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Actividades_Semestral.Models;

public partial class Role
{
    public int IdRol { get; set; }

    public string NombreRol { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
