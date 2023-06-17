using System.ComponentModel.DataAnnotations;
using keevotec.Contracts.Movimento;

namespace keevotec.Validation.Movimento;

public class ValidationUpdateMovimento
{
    public void ValidateUpdateMovimento(UpdateMovimentoRequest request)
    {
	if (request.Ajuste == null && request.DataLancamento == null && string.IsNullOrEmpty(request.Descricao))
	{
	    throw new ValidationException("Ao menos um campo deve ter valor não nulo.");
	}

        if (request.Descricao?.Length > 40)
	{
	    throw new ValidationException("A descrição não pode ser maior que 40 caracteres.");
	}

	if (request.DataLancamento != null && request.DataLancamento?.Kind != DateTimeKind.Utc)
	{
	    throw new ValidationException("Campos de data/hora precisam ser UTC.");
	}
    }
}
