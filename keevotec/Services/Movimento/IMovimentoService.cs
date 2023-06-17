using keevotec.Contracts.Movimento;

namespace keevotec.Services.Movimento;

public interface IMovimentoService
{
    Task<MovimentoResponse> CreateMovimento(CreateMovimentoRequest request);
    Task<MovimentoResponse> GetMovimento(int id);
    Task<List<MovimentoResponse>> GetMovimentos(GetMovimentosRequest request);
    Task DeleteMovimento(int id);
    Task UpdateMovimento(int id, UpdateMovimentoRequest request);
}
