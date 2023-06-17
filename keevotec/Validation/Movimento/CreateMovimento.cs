using System.ComponentModel.DataAnnotations;
using keevotec.Contracts.Movimento;

namespace keevotec.Validation.Movimento;

public class ValidationCreateMovimento
{
    public void ValidateCreateMovimento(CreateMovimentoRequest request)
    {
        if (request.Descricao.Length > 40)
	{
	    throw new ValidationException("A descrição não pode ser maior que 40 caracteres.");
	}
    }
}