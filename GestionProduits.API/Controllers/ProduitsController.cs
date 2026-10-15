using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GestionProduits.API.Models.DTO;

[Route("api/[controller]")]
[ApiController]
public class ProduitsController : ControllerBase
{
    // private readonly GestionProduitsContext _context;
    private readonly IProduitRepository _repository;

    public ProduitsController(IProduitRepository repository)
    {
        _repository = repository;
    }

    // GET: api/Produit
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll()
    {
        var produits = await _repository.GetAllAsync();

        return Ok(produits);
    }

    // GET: api/Produit/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProduitDto>> GetById(int id)
    {
        var produit = await _repository.GetByIdAsync(id);

        if (produit == null)
        {
            return NotFound();
        }

        return Ok(produit);
    }

    // GET: api/Produit/marque/5
    [HttpGet("marque/{idMarque:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetByMarque(int idMarque)
    {
        var produits = await _repository.GetByMarqueAsync(idMarque);

        return Ok(produits);
    }

    // GET: api/produits/recherche?name=ordinateur
    [HttpGet("recherche")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> SearchByName(
        [FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Le terme de recherche est obligatoire.");
        }

        var produits = await _repository.SearchByNameAsync(name);

        return Ok(produits);
    }

    // PUT: api/Produit/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{produitid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutProduit(int produitid, ProduitDto produit)
    {
        if (produitid != produit.Id)
        {
            return BadRequest("L'identifiant de l'URL ne correspond pas au produit.");
        }

        var existingProduit = await _repository.GetByIdAsync(produitid);

        if (existingProduit == null)
        {
            return NotFound();
        }

        await _repository.UpdateAsync(existingProduit, produit);
        await _repository.SaveAsync();

        return NoContent();
    }

    // POST: api/Produit
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProduitDto>> PostProduit(ProduitDto produit)
    {
        await _repository.AddAsync(produit);
        await _repository.SaveAsync();

        return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
    }

    // DELETE: api/Produit/5
    [HttpDelete("{produitid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduit(int produitid)
    {
        var produit = await _repository.GetByIdAsync(produitid);

        if (produit == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(produit);
        await _repository.SaveAsync();

        return NoContent();
    }

    /*
    private bool ProduitExists(int produitid)
    {
        return _context.Produits.Any(e => e.ProduitId == produitid);
    }
    */
}
