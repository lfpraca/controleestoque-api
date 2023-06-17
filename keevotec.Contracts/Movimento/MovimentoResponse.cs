namespace keevotec.Contracts.Movimento;

public record MovimentoResponse(
    int Id,
    int ProdutoId,
    string ProdutoDescricao,
    double Ajuste,
    double Atual,
    string UM,
    string Descricao,
    int Sequencia,
    DateTime DataLancamento
);
