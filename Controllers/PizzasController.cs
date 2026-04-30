using Microsoft.AspNetCore.Mvc;
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
        if (!await _repository.DeleteAsync(id)) return NotFound();
        return NoContent();
    }
}
