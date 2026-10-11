using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<Produit>>> GetAll()
    {
        var produits = await _repository.GetAllAsync();

        return Ok(produits);
    }

    // GET: api/Produit/5
    [HttpGet("{produitid}")]
    public async Task<ActionResult<Produit>> GetById(int id)
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
    public async Task<ActionResult<IEnumerable<Produit>>> GetByMarque(
        int idMarque)
    {
        var produits = await _repository.GetByMarqueAsync(idMarque);

        return Ok(produits);
    }

    // GET: api/produits/recherche?name=ordinateur
    [HttpGet("recherche")]
    public async Task<ActionResult<IEnumerable<Produit>>> SearchByName(
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
    public async Task<ActionResult> PutProduit(int produitid, Produit produit)
    {
        if (produitid != produit.ProduitId)
        {
            return BadRequest(
                "L'identifiant de l'URL ne correspond pas au produit.");
        }

        var existingProduit = await _repository.GetByIdAsync(produitid);

        if (existingProduit == null)
        {
            return NotFound();
        }

        await _repository.UpdateAsync(existingProduit, produit);

        return NoContent();
    }

    // POST: api/Produit
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Produit>> PostProduit(Produit produit)
    {
        await _repository.AddAsync(produit);

        return CreatedAtAction(nameof(GetById), new { id = produit.ProduitId }, produit);
    }

    // DELETE: api/Produit/5
    [HttpDelete("{produitid}")]
    public async Task<IActionResult> DeleteProduit(int produitid)
    {
        var produit = await _repository.GetByIdAsync(produitid);

        if (produit == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(produit);

        return NoContent();
    }

    /*
    private bool ProduitExists(int produitid)
    {
        return _context.Produits.Any(e => e.ProduitId == produitid);
    }
    */
}
