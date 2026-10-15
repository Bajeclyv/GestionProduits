using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GestionProduits.API.Controllers;

public abstract class GenericCrudController<TDto> : ControllerBase
    where TDto : class, IEntityDto
{
    private readonly IDataRepository<TDto> _repository;

    protected GenericCrudController(IDataRepository<TDto> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TDto>> GetById(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TDto>> Post(TDto entity)
    {
        await _repository.AddAsync(entity);
        await _repository.SaveAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, TDto entity)
    {
        if (id != entity.Id)
        {
            return BadRequest("L'identifiant de l'URL ne correspond pas à l'entité.");
        }

        var existingEntity = await _repository.GetByIdAsync(id);
        if (existingEntity == null)
        {
            return NotFound();
        }

        await _repository.UpdateAsync(existingEntity, entity);
        await _repository.SaveAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(entity);
        await _repository.SaveAsync();
        return NoContent();
    }
}
