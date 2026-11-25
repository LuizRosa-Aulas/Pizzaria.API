namespace Pizzaria.API.Models;

public class VendaAcompanhamento
{
	public int Id { get; set; }
	public int VendaId { get; set; }
	public int AcompanhamentoId { get; set; }
	public int Quantidade { get; set; }
	public decimal PrecoUnitario { get; set; }

	public string? NomeAcompanhamento { get; set; }
}