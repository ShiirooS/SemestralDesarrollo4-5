namespace Actividades_Semestral.Models
{
    public class ActividadDetalles
    {
        public Dictionary<string, object> InfoActividad { get; set; }
        public List<dynamic> Usuarios { get; set; }


        public ActividadDetalles()
        {
            InfoActividad = new Dictionary<string, object>();
            Usuarios = new List<dynamic>();
        }
    }
}
