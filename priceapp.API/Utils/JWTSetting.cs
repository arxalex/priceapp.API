using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace priceapp.API.Utils;

public class JWTSetting
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Lifetime { get; set; }

    public SecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    }
}