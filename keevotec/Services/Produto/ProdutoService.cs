using keevotec.Contracts.Movimento;
using keevotec.Contracts.Produto;
using keevotec.Data;
using keevotec.Exceptions;
using keevotec.Services.Movimento;
using Microsoft.EntityFrameworkCore;

namespace keevotec.Services.Produto;

public class ProdutoService : IProdutoService
{
    private readonly KeevotecContext _dbContext;
    private readonly IMovimentoService _movimentoService;

    public ProdutoService(KeevotecContext dbContext, IMovimentoService movimentoService)
    {
        _dbContext = dbContext;
	_movimentoService = movimentoService;
    }

    public async Task<ProdutoResponse> CreateProduto(CreateProdutoRequest request)
    {
        var produto = new Models.Produto
	{
	    Descricao = request.Descricao,
	    UM = request.UM
	};
        _dbContext.Add(produto);
	await _dbContext.SaveChangesAsync();

	_ = await _movimentoService.CreateMovimento(
	    new CreateMovimentoRequest(
		ProdutoId: produto.Id,
		Ajuste: request.EstoqueInicial,
		Descricao: "Estoque Inicial",
		DataLancamento: DateTime.UtcNow));

	return new ProdutoResponse(produto.Id, produto.Descricao, produto.UM);
    }

    public async Task DeleteProduto(int id)
    {
	var produto = _dbContext.Produtos.Find(id);
	if (produto != null)
	{
	    _dbContext.Produtos.Remove(produto);
	    await _dbContext.SaveChangesAsync();
	}
	else
	{
	    throw new NotFoundException($"Produto com id {id} não encontrado.");
	}
    }

    public async Task<ProdutoResponse> GetProduto(int id)
    {
	var produto = await _dbContext.Produtos.FindAsync(id);

        if (produto != null)
	{
	    var response = new ProdutoResponse(produto.Id, produto.Descricao, produto.UM);
	    return(response);
	}
	else
	{
	    throw new NotFoundException($"Produto com id {id} não encontrado.");
	}
    }

    public async Task<List<ProdutoResponse>> GetProdutos()
    {
	var produtos = await _dbContext.Produtos
	    .Select(p => new ProdutoResponse(p.Id, p.Descricao, p.UM))
	    .ToListAsync();
	
	return(produtos);
    }

    public async Task UpdateProduto(int id, UpdateProdutoRequest request)
    {
	var produto = await _dbContext.Produtos.FindAsync(id);

	if (produto != null)
	{
	    if (request.Descricao != null)
	    {
		produto.Descricao = request.Descricao;
	    }
	    if (request.UM != null)
	    {
		produto.UM = request.UM;
	    }
	    await _dbContext.SaveChangesAsync();
	}
	else
	{
	    throw new NotFoundException($"Produto com id {id} não encontrado.");
	}
    }
}
