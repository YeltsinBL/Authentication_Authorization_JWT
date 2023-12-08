using System;
using System.Collections.Generic;

namespace LoginToken.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public byte[] Clave { get; set; }

    public byte[] ClaveSalt { get; set; }

    public string? VerificarToken { get; set; }

    public DateTime? Verificar { get; set; }

    public string? ClaveResetToken { get; set; }

    public DateTime? ResetTokenExpires { get; set; }

    public virtual ICollection<HistorialRefreshToken> HistorialRefreshTokens { get; set; } = new List<HistorialRefreshToken>();
}
