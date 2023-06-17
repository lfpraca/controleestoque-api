using System.ComponentModel.DataAnnotations;
using keevotec.Contracts.Produto;

namespace keevotec.Validation.Produto;

public class ValidationUpdateProduto
{
    public void ValidateUpdateProduto(UpdateProdutoRequest request)
    {
	if (string.IsNullOrEmpty(request.UM) && string.IsNullOrEmpty(request.Descricao))
	{
	    throw new ValidationException("Ao menos um campo deve ter valor não nulo.");
	}

        if (request.Descricao?.Length > 40)
	{
	    throw new ValidationException("A descrição não pode ser maior que 40 caracteres.");
	}

        if (request.UM?.Length > 5)
	{
	    throw new ValidationException("A unidade de medida não pode ser maior que 5 caracteres.");
	}
    }
}
