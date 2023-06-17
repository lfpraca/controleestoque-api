namespace keevotec.Contracts.Movimento;

public record UpdateMovimentoRequest(
    double? Ajuste,
    string? Descricao,
    DateTime? DataLancamento
);
