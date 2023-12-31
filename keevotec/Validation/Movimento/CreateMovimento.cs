using System.ComponentModel.DataAnnotations;
using keevotec.Contracts.Movimento;

namespace keevotec.Validation.Movimento;

public class ValidationCreateMovimento
{
    public void ValidateCreateMovimento(CreateMovimentoRequest request)
    {
	if (request.Ajuste == 0)
	{
	    throw new ValidationException("O valor do ajuste deve ser diferente de 0.");
	}

        if (request.Descricao.Length > 40)
	{
	    throw new ValidationException("A descrição não pode ser maior que 40 caracteres.");
	}

	if (request.DataLancamento.Kind != DateTimeKind.Utc)
	{
	    throw new ValidationException("Campos de data/hora precisam ser UTC.");
	}
    }
}
