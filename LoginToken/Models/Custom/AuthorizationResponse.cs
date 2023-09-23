namespace LoginToken.Models.Custom
{
    // Respuesta al Login
    public class AuthorizationResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Resultado { get; set; }
        public string Msg { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
    }
}
