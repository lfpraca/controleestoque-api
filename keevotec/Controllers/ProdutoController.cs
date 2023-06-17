using keevotec.Contracts.Produto;
using keevotec.Services.Produto;
using keevotec.Validation.Produto;
using Microsoft.AspNetCore.Mvc;

namespace keevotec.Controllers;

[ApiController]
[Route("produto")]
public class ProdutoController : ControllerBase
{
    private readonly ValidationCreateProduto _validationCreateProduto;
    private readonly ValidationUpdateProduto _validationUpdateProduto;
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        _validationCreateProduto = new ValidationCreateProduto();
        _validationUpdateProduto = new ValidationUpdateProduto();
	_produtoService = produtoService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduto(CreateProdutoRequest request)
    {
	_validationCreateProduto.ValidateCreateProduto(request);
	var response = await _produtoService.CreateProduto(request);
	
        return CreatedAtAction(nameof(GetProduto),
			       new { id = response.Id },
			       response);
    }

    [HttpGet]
    public async Task<IActionResult> GetProdutos()
    {
	var response = await _produtoService.GetProdutos();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduto(int id)
    {
	var response = await _produtoService.GetProduto(id);
	return Ok(response);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateProduto(int id, UpdateProdutoRequest request)
    {
	_validationUpdateProduto.ValidateUpdateProduto(request);

	await _produtoService.UpdateProduto(id, request);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduto(int id)
    {
	await _produtoService.DeleteProduto(id);
	return NoContent();
    }
}
