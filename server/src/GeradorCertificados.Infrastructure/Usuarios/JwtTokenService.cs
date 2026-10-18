using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeradorCertificados.Application.Usuarios;
using GeradorCertificados.Domain.Usuarios;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GeradorCertificados.Infrastructure.Usuarios;

public sealed class JwtTokenService(IConfiguration config) : ITokenService
{
    public TokenDto Gerar(Usuario usuario)
    {
        var settings = config.GetRequiredSection("Jwt").Get<JwtSettings>()!;
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));
        var dataExpiracaoEmUtc = DateTimeOffset.UtcNow.AddMinutes(settings.ExpiresInMinutes);
        var token = new JwtSecurityToken(
            settings.Issuer,
            settings.Audience,
            [
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
            ],
            expires: dataExpiracaoEmUtc.UtcDateTime,
            signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenDto(usuario.Id, accessToken, dataExpiracaoEmUtc);
    }
}
