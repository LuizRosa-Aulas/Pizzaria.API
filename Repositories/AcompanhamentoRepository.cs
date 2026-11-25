
using MySqlConnector;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class AcompanhamentoRepository
{
    private readonly string _connectionString;

    public AcompanhamentoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private async Task<MySqlConnection> CreateConnectionAsync()
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public async Task<List<Acompanhamento>> GetAllAsync()
    {
        var Acompanhamentos = new List<Acompanhamento>();

        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Descricao, Categoria, Preco FROM Acompanhamentos";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Acompanhamentos.Add(new Acompanhamento
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.GetString(2),
                Categoria = reader.GetString(3),
                Preco = reader.GetDecimal(4)
            });
        }

        return Acompanhamentos;
    }

    public async Task<Acompanhamento?> GetByIdAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Descricao, Categoria, Preco FROM Acompanhamentos WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Acompanhamento
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.GetString(2),
                Categoria = reader.GetString(3),
                Preco = reader.GetDecimal(4)
            };
        }

        return null;
    }

    public async Task<Acompanhamento> CreateAsync(Acompanhamento Acompanhamento)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Acompanhamentos (Nome, Descricao, Categoria, Preco)
            VALUES (@Nome, @Descricao, @Categoria, @Preco)";

        command.Parameters.AddWithValue("@Nome", Acompanhamento.Nome);
        command.Parameters.AddWithValue("@Descricao", Acompanhamento.Descricao);
        command.Parameters.AddWithValue("@Categoria", Acompanhamento.Categoria);
        command.Parameters.AddWithValue("@Preco", Acompanhamento.Preco);

        await command.ExecuteNonQueryAsync();

        Acompanhamento.Id = (int)command.LastInsertedId;
        return Acompanhamento;
    }

    public async Task<bool> UpdateAsync(Acompanhamento Acompanhamento)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Acompanhamentos
            SET Nome = @Nome, Descricao = @Descricao, Categoria = @Categoria, Preco = @Preco
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", Acompanhamento.Id);
        command.Parameters.AddWithValue("@Nome", Acompanhamento.Nome);
        command.Parameters.AddWithValue("@Descricao", Acompanhamento.Descricao);
        command.Parameters.AddWithValue("@Categoria", Acompanhamento.Categoria);
        command.Parameters.AddWithValue("@Preco", Acompanhamento.Preco);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Acompanhamentos WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
