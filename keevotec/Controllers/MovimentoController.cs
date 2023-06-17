using keevotec.Contracts.Movimento;
using keevotec.Services.Movimento;
using keevotec.Validation.Movimento;
using Microsoft.AspNetCore.Mvc;

namespace keevotec.Controllers;

[ApiController]
[Route("movimento")]
public class MovimentoController : ControllerBase
{
    private readonly ValidationCreateMovimento _validationCreateMovimento;
    private readonly ValidationUpdateMovimento _validationUpdateMovimento;
    private readonly IMovimentoService _movimentoService;

    public MovimentoController(IMovimentoService movimentoService)
    {
        _validationCreateMovimento = new ValidationCreateMovimento();
        _validationUpdateMovimento = new ValidationUpdateMovimento();
	_movimentoService = movimentoService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovimento(CreateMovimentoRequest request)
    {
	_validationCreateMovimento.ValidateCreateMovimento(request);
	var response = await _movimentoService.CreateMovimento(request);

	return CreatedAtAction(nameof(GetMovimento),
		       new { id = response.Id },
		       response);
    }

    [HttpGet]
    public async Task<IActionResult> GetMovimentos(GetMovimentosRequest request)
    {
	var response = await _movimentoService.GetMovimentos(request);

	return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMovimento(int id)
    {
	var response = await _movimentoService.GetMovimento(id);

	return Ok(response);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateMovimento(int id, UpdateMovimentoRequest request)
    {
	    _validationUpdateMovimento.ValidateUpdateMovimento(request);
	    await _movimentoService.UpdateMovimento(id, request);

	    return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovimento(int id)
    {
	await _movimentoService.DeleteMovimento(id);

	return NoContent();
    }
}
