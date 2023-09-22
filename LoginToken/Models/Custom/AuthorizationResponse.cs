namespace LoginToken.Models.Custom
{
    // Respuesta al Login
    public class AuthorizationResponse
    {
        public string Token { get; set; }
        public bool Resultado { get; set; }
        public string Msg { get; set; }
    }
}
