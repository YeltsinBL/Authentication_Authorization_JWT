using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace LoginBlazorWeb.Extensiones
{
    public class Utility
    {
        // Leer el Token
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            //return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

            //keyValuePairs!.TryGetValue("role", out object roles);
            keyValuePairs!.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString()!.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);

                    foreach (var parsedRole in parsedRoles!)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                }

                //keyValuePairs.Remove("role");
                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
            return claims;
            //return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));

        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
        /// <summary>
        /// <para>
        /// Método para encriptar datos con SHA256
        /// </para>
        /// </summary>
        /// <param name="valor">Un valor de tipo cadena</param>
        /// <returns>
        /// Este metodo devuelve un dato encriptado
        /// </returns>
        public static string GetSHA256(string valor)
        {
            ASCIIEncoding aSCIIEncoding = new();
            StringBuilder stringBuilder = new();
            byte[] stream = SHA256.HashData(aSCIIEncoding.GetBytes(valor));
            for (int i = 0; i < stream.Length; i++) stringBuilder.AppendFormat("{0:x2}", stream[i]);

            return stringBuilder.ToString();

        }

        /// <summary>
        /// <para>
        /// Método para obtener el RequestUri del appsettings
        /// </para>
        /// </summary>
        /// <param name="configuration">La interfaz de configuración de la aplicación</param>
        /// <param name="name_path">Parte final del RequestUri</param>
        /// <param name="nivel">Nivel de llaves en el json</param>
        /// <returns>
        /// Este metodo devuelve el RequestUri de acuerdo al path final
        /// </returns>
        public static string GetRequestUri(IConfiguration configuration, string name_path, int nivel=1)
        {
            if (nivel == 2)
                return configuration.GetValue<string>("httpClient:requestUri:" + name_path);
            return configuration.GetValue<string>("httpClient:" + name_path);
        }
    }
}
