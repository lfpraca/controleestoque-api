using System.ComponentModel.DataAnnotations;
using keevotec.Contracts.Produto;

namespace keevotec.Validation.Produto;

public class ValidationCreateProduto
{
    public void ValidateCreateProduto(CreateProdutoRequest request)
    {
        if (request.Descricao.Length > 40)
	{
	    throw new ValidationException("A descrição não pode ser maior que 40 caracteres.");
	}

        if (request.UM.Length > 5)
	{
	    throw new ValidationException("A unidade de medida não pode ser maior que 5 caracteres.");
	}
    }
}
