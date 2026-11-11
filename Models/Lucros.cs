namespace Pizzaria.API.Models;

public class Lucros
{
    public int VendaId { get; set; }
    public decimal CustoTotal { get; set; }
    public decimal VendasValorTotal { get; set; }
    public decimal Lucro { get; set; }
}
