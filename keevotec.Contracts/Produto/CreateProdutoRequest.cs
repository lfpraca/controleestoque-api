namespace keevotec.Contracts.Produto;

public record CreateProdutoRequest
(
    string Descricao,
    string UM,
    double EstoqueInicial
);
