using Microsoft.AspNetCore.Mvc;
using Pizzaria.API.Models;
using Pizzaria.API.Repositories;

namespace Pizzaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly VendaRepository _repository;

    public VendasController(VendaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Venda>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Venda>> GetById(int id)
    {
        var venda = await _repository.GetByIdAsync(id);
        if (venda == null) return NotFound();
        return Ok(venda);
    }

    [HttpPost]
    public async Task<ActionResult<Venda>> Create(Venda venda)
    {
        var created = await _repository.CreateAsync(venda);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Venda venda)
    {
        venda.Id = id;
        if (!await _repository.UpdateAsync(venda)) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!await _repository.DeleteAsync(id)) return NotFound();
        return NoContent();
    }
}
