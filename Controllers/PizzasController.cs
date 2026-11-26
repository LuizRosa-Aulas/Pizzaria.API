using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Pizzaria.API.Models;
using Pizzaria.API.Repositories;

namespace Pizzaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
    private readonly PizzaRepository _repository;

    public PizzasController(PizzaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Pizza>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pizza>> GetById(int id)
    {
        var pizza = await _repository.GetByIdAsync(id);
        if (pizza == null) return NotFound();
        return Ok(pizza);
    }

    [HttpPost]
    public async Task<ActionResult<Pizza>> Create(Pizza pizza)
    {
        var created = await _repository.CreateAsync(pizza);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Pizza pizza)
    {
        pizza.Id = id;
        if (!await _repository.UpdateAsync(pizza)) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            if (!await _repository.DeleteAsync(id)) return NotFound();
            return NoContent();
        }
        catch (MySqlException ex) when (ex.ErrorCode is MySqlErrorCode.RowIsReferenced2
                                          or MySqlErrorCode.RowIsReferenced)
        {
            // O banco barra a exclusão por chave estrangeira: responde 409 com o motivo.
            // 1451 ou 1217: o MySQL usa o 1217 quando a conta não pode ver a tabela filha.
            return Conflict(new { mensagem = "Esta pizza tem vendas registradas e não pode ser excluída." });
        }
    }

    [HttpGet("tipo")]
    public async Task<ActionResult<List<Tipo>>> GetAllTipo()
    {
        return Ok(await _repository.GetAllTipoAsync());
    }
    [HttpGet("borda")]
    public async Task<ActionResult<List<Borda>>> GetAllBorda()
    {
        return Ok(await _repository.GetAllBordaAsync());
    }
}
