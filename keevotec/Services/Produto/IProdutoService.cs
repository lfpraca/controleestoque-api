using keevotec.Contracts.Produto;

namespace keevotec.Services.Produto;

public interface IProdutoService
{
    Task<ProdutoResponse> CreateProduto(CreateProdutoRequest request);
    Task<ProdutoResponse> GetProduto(int id);
    Task<List<ProdutoResponse>> GetProdutos();
    Task DeleteProduto(int id);
    Task UpdateProduto(int id, UpdateProdutoRequest request);
}
