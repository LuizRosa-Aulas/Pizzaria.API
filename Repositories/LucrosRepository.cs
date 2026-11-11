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
            SELECT l.VendaId, l.CustoTotal, l.VendasValorTotal, l.Lucro
            FROM Lucros l
            INNER JOIN Vendas v ON v.Id = l.VendaId
            INNER JOIN Vendas v ON v.ValorTotal = l.VendasValorTotal
            ORDER BY v.Id DESC";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lucros.Add(new Lucros
            {
                VendaId = reader.GetInt32(0),
                CustoTotal = reader.GetDecimal(1),
                VendasValorTotal = reader.GetDecimal(2),
                Lucro = reader.GetDecimal(3),

            });
        }

        return lucros;
    }

    public async Task<Lucros?> GetByIdAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT l.VendaId, l.CustoTotal, l.VendasValorTotal, l.Lucro
            FROM Lucros l
            INNER JOIN Vendas v ON v.Id = l.VendaId
            INNER JOIN Vendas v ON v.ValorTotal = l.VendasValorTotal
            ORDER BY v.Id DESC";

        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Lucros
            {
                VendaId = reader.GetInt32(0),
                CustoTotal = reader.GetDecimal(1),
                VendasValorTotal = reader.GetDecimal(2),
                Lucro = reader.GetDecimal(3),
            };
        }

        return null;
    }
}
