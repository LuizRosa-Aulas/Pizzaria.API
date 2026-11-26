using MySqlConnector;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class LucrosRepository
{
    private readonly string _connectionString;

    public LucrosRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private async Task<MySqlConnection> CreateConnectionAsync()
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }


    public async Task<List<Lucros>> GetAllAsync()
    {
        var lucros = new List<Lucros>();

        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, ValorTotal, CustoTotal, DataVenda
            FROM Vendas";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lucros.Add(new Lucros
            {
                VendaId = reader.GetInt32(0),
                VendasValorTotal = reader.GetDecimal(1),
                CustoTotal = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2),
                DataVenda = reader.GetDateTime(3),
                Lucro = reader.GetDecimal(1) - (reader.IsDBNull(2) ? 0m : reader.GetDecimal(2))
            });
        }

        return lucros;
    }
}
