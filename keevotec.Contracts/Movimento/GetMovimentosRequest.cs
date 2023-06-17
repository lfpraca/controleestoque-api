namespace keevotec.Contracts.Movimento;

public record GetMovimentosRequest(
    int? ProdutoId,
    DateTime? DataInicio,
    DateTime? DataFim
);
