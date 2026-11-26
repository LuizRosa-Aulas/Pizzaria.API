namespace Pizzaria.API.Models;

public class Pizza
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Custo { get; set; } 
    public decimal Preco { get; set; }
    public string Obs { get; set; } = string.Empty;
    public int Qtd { get; set; }
}
