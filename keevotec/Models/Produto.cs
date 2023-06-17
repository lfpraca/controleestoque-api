using System.ComponentModel.DataAnnotations;

namespace keevotec.Models;

public class Produto
{
    public int Id { get; set; }
    [MaxLength(40)]
    public string Descricao { get; set; } = null!;
    [MaxLength(5)]
    public string UM { get; set; } = null!;
    public ICollection<Movimento> Movimentos { get; set; } = null!;
}
