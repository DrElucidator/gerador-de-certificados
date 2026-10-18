using GeradorCertificados.Api.Contracts.Auth;
using GeradorCertificados.Application.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificados.Api.Controllers;
[ApiController]
[Route("auth")]
[AllowAnonymous]
public sealed class AuthController(AutenticacaoService service) : ControllerBase
{
    [HttpPost("cadastro")]
    public async Task<ActionResult<UsuarioDto>> Cadastro(CadastroRequest request, CancellationToken ct)
    {
        try
        {
            var usuario = await service.CadastrarAsync(request.Email, request.Senha, ct);

            return StatusCode(StatusCodes.Status201Created, usuario);
        }
        catch (ConflitoDeNegocioException exception)
        {
            return Conflict(new ProblemDetails { Title = "Conflito", Detail = exception.Message, Status = 409 });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Dados inválidos", Detail = exception.Message, Status = 400 });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginRequest request, CancellationToken ct)
    {
        try
        {
            var token = await service.LoginAsync(request.Email, request.Senha, ct);

            return Ok(token);
        }
        catch (CredenciaisInvalidasException exception)
        {
            return Unauthorized(new ProblemDetails { Title = "Não autorizado", Detail = exception.Message, Status = 401 });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Dados inválidos", Detail = exception.Message, Status = 400 });
        }
    }
}
