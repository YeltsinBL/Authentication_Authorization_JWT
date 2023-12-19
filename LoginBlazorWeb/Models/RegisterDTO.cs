using System.ComponentModel.DataAnnotations;

namespace LoginBlazorWeb.Models
{
    public class RegisterDTO
    {
        [Required, DataType(DataType.EmailAddress,ErrorMessage ="Debe ingresar un correo electrónico válido")]
        public string usuario { get; set; }
        [Required]
        public string password { get; set; }
        [Compare("password", ErrorMessage ="Las contraseñas deben ser iguales.")]
        public string confirmPassword { get; set; }
    }
}
