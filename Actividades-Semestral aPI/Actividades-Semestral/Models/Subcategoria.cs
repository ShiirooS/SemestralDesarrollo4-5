using System;
using System.Collections.Generic;

namespace Actividades_Semestral.Models;

public partial class Subcategoria
{
    public int IdSubcategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public int? IdCategoria { get; set; }

    public virtual ICollection<Actividade> Actividades { get; set; } = new List<Actividade>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<PropuestasActividade> PropuestasActividades { get; set; } = new List<PropuestasActividade>();
}
