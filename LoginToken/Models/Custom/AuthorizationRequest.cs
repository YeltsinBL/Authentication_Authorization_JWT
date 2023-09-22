namespace LoginToken.Models.Custom
{
    // Recibe las credenciales del Login
    public class AuthorizationRequest
    {
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
    }
}
