using System;
using System.Collections.Generic;

namespace Actividades_Semestral.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Facultad { get; set; }

    public int? IdRol { get; set; }

    public string? contrasena { get; set; }
    public virtual Role? IdRolNavigation { get; set; }

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();

    public virtual ICollection<Notificacione> Notificaciones { get; set; } = new List<Notificacione>();

    public virtual ICollection<PropuestasActividade> PropuestasActividades { get; set; } = new List<PropuestasActividade>();
}
