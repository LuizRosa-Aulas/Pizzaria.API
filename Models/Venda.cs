namespace Pizzaria.API.Models;

public class Venda
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int PizzaId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataVenda { get; set; }
    public string? StatusId { get; set; }

    // Dados preenchidos nas consultas (JOIN)
    public string? NomeUsuario { get; set; }
    public string? NomePizza { get; set; }
    public string? NomeStatus { get; set; }
}