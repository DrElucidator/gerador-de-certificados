using GeradorCertificados.Api.Contracts.Cursos;
using GeradorCertificados.Application.Cursos;
using GeradorCertificados.Domain.Cursos;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificados.Api.Controllers;

[ApiController]
[Route("cursos")]
public sealed class CursosController(ServicoCursos service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CursoResponse>>> Listar(CancellationToken ct)
    {
        var cursos = await service.ListarAsync(ct);
        return Ok(cursos.Select(ParaResposta).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar(CadastrarCursoRequest request, CancellationToken ct)
    {
        try
        {
            var curso = await service.CadastrarAsync(
                request.Nome,
                request.Descricao,
                request.CargaHoraria,
                request.DataConclusao,
                ct);

            return CreatedAtAction(nameof(Consultar), new { cursoId = curso.Id }, ParaResposta(curso));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Dados inválidos",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest,
            });
        }
    }

    [HttpGet("{cursoId:guid}")]
    public async Task<ActionResult<CursoResponse>> Consultar(Guid cursoId, CancellationToken ct)
    {
        var curso = await service.ConsultarAsync(cursoId, ct);
        return curso is null ? NotFound() : Ok(ParaResposta(curso));
    }

    private static CursoResponse ParaResposta(Curso curso)
    {
        return new CursoResponse(curso.Id, curso.Nome, curso.Descricao, curso.CargaHoraria, curso.DataConclusao);
    }
}
