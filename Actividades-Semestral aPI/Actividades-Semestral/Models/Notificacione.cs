using System;
using System.Collections.Generic;

namespace Actividades_Semestral.Models;

public partial class Notificacione
{
    public int IdNotificacion { get; set; }

    public int? IdUsuario { get; set; }

    public string Mensaje { get; set; } = null!;

    public DateTime? FechaNotificacion { get; set; }

    public bool? Leido { get; set; }

    public string TipoNotificacion { get; set; } = null!;

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
