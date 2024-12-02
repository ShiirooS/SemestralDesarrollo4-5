namespace Actividades_Semestral.Models
{
    public class LoginRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int RolId { get; set; }
    }

}
