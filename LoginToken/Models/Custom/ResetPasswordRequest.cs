using System.ComponentModel.DataAnnotations;

namespace LoginToken.Models.Custom
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Usuario { get; set; } = string.Empty;
        [Required, MinLength(6, ErrorMessage = "Ingrese al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
