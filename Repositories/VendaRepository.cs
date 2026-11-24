
using MySqlConnector;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class VendaRepository
{
    private readonly string _connectionString;

    public VendaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private async Task<MySqlConnection> CreateConnectionAsync()
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public async Task<List<Venda>> GetAllAsync()
    {
        var vendas = new List<Venda>();

        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT v.Id, v.UsuarioId, v.PizzaId, v.Quantidade, v.ValorTotal, v.DataVenda,
                   u.Nome AS NomeUsuario, p.Nome AS NomePizza,
                   v.StatusId, s.Nome AS NomeStatus
            FROM Vendas v
            INNER JOIN Usuarios u ON u.Id = v.UsuarioId
            INNER JOIN Pizzas p ON p.Id = v.PizzaId
            INNER JOIN Status s ON s.StatusId = v.StatusId
            ORDER BY v.DataVenda DESC";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            command.CommandText = @"
                SELECT v.Id, v.UsuarioId, v.PizzaId, v.Quantidade, v.ValorTotal, v.DataVenda,
                       u.Nome AS NomeUsuario, p.Nome AS NomePizza
                FROM Vendas v
                INNER JOIN Usuarios u ON u.Id = v.UsuarioId
                INNER JOIN Pizzas p ON p.Id = v.PizzaId
                ORDER BY v.DataVenda DESC";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                vendas.Add(new Venda
                {
                    Id = reader.GetInt32(0),
                    UsuarioId = reader.GetInt32(1),
                    PizzaId = reader.GetInt32(2),
                    Quantidade = reader.GetInt32(3),
                    ValorTotal = reader.GetDecimal(4),
                    DataVenda = reader.GetDateTime(5),
                    NomeUsuario = reader.GetString(6),
                    NomePizza = reader.GetString(7)
                });
            }
        }

        foreach (var venda in vendas)
        {
            await CarregarAcompanhamentosAsync(venda, connection);
        }

        return vendas;
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        Venda? venda = null;

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
                SELECT v.Id, v.UsuarioId, v.PizzaId, v.Quantidade, v.ValorTotal, v.DataVenda,
                       u.Nome AS NomeUsuario, p.Nome AS NomePizza
                FROM Vendas v
                INNER JOIN Usuarios u ON u.Id = v.UsuarioId
                INNER JOIN Pizzas p ON p.Id = v.PizzaId
                WHERE v.Id = @Id";

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                venda = new Venda
                {
                    Id = reader.GetInt32(0),
                    UsuarioId = reader.GetInt32(1),
                    PizzaId = reader.GetInt32(2),
                    Quantidade = reader.GetInt32(3),
                    ValorTotal = reader.GetDecimal(4),
                    DataVenda = reader.GetDateTime(5),
                    NomeUsuario = reader.GetString(6),
                    NomePizza = reader.GetString(7)
                };
            }
        }

        if (venda != null)
        {
            await CarregarAcompanhamentosAsync(venda, connection);
        }

        return venda;
    }

    private async Task CarregarAcompanhamentosAsync(Venda venda, MySqlConnection connection)
    {
        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT va.Id, va.VendaId, va.AcompanhamentoId,
                   va.Quantidade, va.PrecoUnitario, a.Nome
            FROM VendaAcompanhamentos va
            INNER JOIN Acompanhamentos a ON a.ID = va.AcompanhamentoId
            WHERE va.VendaId = @VendaId";

        command.Parameters.AddWithValue("@VendaId", venda.Id);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            venda.Acompanhamentos.Add(new VendaAcompanhamento
            {
                Id = reader.GetInt32(0),
                VendaId = reader.GetInt32(1),
                AcompanhamentoId = reader.GetInt32(2),
                Quantidade = reader.GetInt32(3),
                PrecoUnitario = reader.GetDecimal(4),
                NomeAcompanhamento = reader.GetString(5)
            });
        }
    }

    public async Task<Venda> CreateAsync(Venda venda)
    {
        using var connection = await CreateConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var dataVenda = DateTime.Now;

            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"
                INSERT INTO Vendas (UsuarioId, PizzaId, Quantidade, ValorTotal, DataVenda)
                VALUES (@UsuarioId, @PizzaId, @Quantidade, @ValorTotal, @DataVenda)";

            command.Parameters.AddWithValue("@UsuarioId", venda.UsuarioId);
            command.Parameters.AddWithValue("@PizzaId", venda.PizzaId);
            command.Parameters.AddWithValue("@Quantidade", venda.Quantidade);
            command.Parameters.AddWithValue("@ValorTotal", venda.ValorTotal);
            command.Parameters.AddWithValue("@DataVenda", dataVenda);

            await command.ExecuteNonQueryAsync();

            venda.Id = (int)command.LastInsertedId;
            venda.DataVenda = dataVenda;

            foreach (var acompanhamento in venda.Acompanhamentos)
            {
                using var itemCommand = connection.CreateCommand();
                itemCommand.Transaction = transaction;

                itemCommand.CommandText = @"
                    INSERT INTO VendaAcompanhamentos
                    (VendaId, AcompanhamentoId, Quantidade, PrecoUnitario)
                    VALUES (@VendaId, @AcompanhamentoId, @Quantidade, @PrecoUnitario)";

                itemCommand.Parameters.AddWithValue("@VendaId", venda.Id);
                itemCommand.Parameters.AddWithValue("@AcompanhamentoId", acompanhamento.AcompanhamentoId);
                itemCommand.Parameters.AddWithValue("@Quantidade", acompanhamento.Quantidade);
                itemCommand.Parameters.AddWithValue("@PrecoUnitario", acompanhamento.PrecoUnitario);

                await itemCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            return venda;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Venda venda)
    {
        using var connection = await CreateConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"
                UPDATE Vendas
                SET UsuarioId = @UsuarioId,
                    PizzaId = @PizzaId,
                    Quantidade = @Quantidade,
                    ValorTotal = @ValorTotal
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", venda.Id);
            command.Parameters.AddWithValue("@UsuarioId", venda.UsuarioId);
            command.Parameters.AddWithValue("@PizzaId", venda.PizzaId);
            command.Parameters.AddWithValue("@Quantidade", venda.Quantidade);
            command.Parameters.AddWithValue("@ValorTotal", venda.ValorTotal);

            var resultado = await command.ExecuteNonQueryAsync();

            if (resultado == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            using var deleteCommand = connection.CreateCommand();
            deleteCommand.Transaction = transaction;
            deleteCommand.CommandText = "DELETE FROM VendaAcompanhamentos WHERE VendaId = @VendaId";
            deleteCommand.Parameters.AddWithValue("@VendaId", venda.Id);

            await deleteCommand.ExecuteNonQueryAsync();

            foreach (var acompanhamento in venda.Acompanhamentos)
            {
                using var itemCommand = connection.CreateCommand();
                itemCommand.Transaction = transaction;

                itemCommand.CommandText = @"
                    INSERT INTO VendaAcompanhamentos
                    (VendaId, AcompanhamentoId, Quantidade, PrecoUnitario)
                    VALUES (@VendaId, @AcompanhamentoId, @Quantidade, @PrecoUnitario)";

                itemCommand.Parameters.AddWithValue("@VendaId", venda.Id);
                itemCommand.Parameters.AddWithValue("@AcompanhamentoId", acompanhamento.AcompanhamentoId);
                itemCommand.Parameters.AddWithValue("@Quantidade", acompanhamento.Quantidade);
                itemCommand.Parameters.AddWithValue("@PrecoUnitario", acompanhamento.PrecoUnitario);

                await itemCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = await CreateConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            using var deleteItems = connection.CreateCommand();
            deleteItems.Transaction = transaction;
            deleteItems.CommandText = "DELETE FROM VendaAcompanhamentos WHERE VendaId = @Id";
            deleteItems.Parameters.AddWithValue("@Id", id);

            await deleteItems.ExecuteNonQueryAsync();

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM Vendas WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            var resultado = await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            return resultado > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<bool> UpdateStatusAsync(int id, int StatusId)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Vendas
            SET StatusId = @StatusId
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@StatusId", StatusId);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
