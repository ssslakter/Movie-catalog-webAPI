using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MovieCatalogAPI.Configurations
{
    public class JwtConfigurations
    {
        public const string Issuer = "MovieDBIssuer"; // издатель токена
        public const string Audience = "MovieDBClient"; // потребитель токена
        private const string Key = "SuperSecretKeyTHISISMYSECRET!!!";   // ключ для шифрации
        public const int Lifetime = 60; // время жизни токена - 60 минут
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }

}
