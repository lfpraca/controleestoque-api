using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace keevotec.Models;

[Index(nameof(Sequencia), nameof(ProdutoId), IsUnique = true)]
public class Movimento
{
    public int Id { get; set; }
    public double Ajuste { get; set; }
    public double Atual { get; set; }
    [MaxLength(40)]
    public string Descricao { get; set; } = null!;
    public DateTime DataHora { get; set; }
    public int Sequencia { get; set; }
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
}
