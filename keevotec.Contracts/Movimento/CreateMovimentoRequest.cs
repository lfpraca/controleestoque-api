namespace keevotec.Contracts.Movimento;

public record CreateMovimentoRequest(
    int ProdutoId,
    double Ajuste,
    string Descricao,
    DateTime DataLancamento
);
