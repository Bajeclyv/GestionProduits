using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionProduits.API.Models.EntityFramework;

[Route("api/[controller]")]
[ApiController]
public class ProduitsController : ControllerBase
{
    private readonly GestionProduitsContext _context;
    public ProduitsController(GestionProduitsContext context)
    {
        _context = context;
    }

    // GET: api/Produit
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produit>>> GetProduit()
    {
        return await _context.Produits.ToListAsync();
    }

    // GET: api/Produit/5
    [HttpGet("{produitid}")]
    public async Task<ActionResult<Produit>> GetProduit(int produitid)
    {
        var produit = await _context.Produits.FindAsync(produitid);

        if (produit == null)
        {
            return NotFound();
        }

        return produit;
    }

    // PUT: api/Produit/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{produitid}")]
    public async Task<IActionResult> PutProduit(int? produitid, Produit produit)
    {
        if (produitid != produit.ProduitId)
        {
            return BadRequest();
        }

        _context.Entry(produit).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProduitExists(produitid))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Produit
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Produit>> PostProduit(Produit produit)
    {
        _context.Produits.Add(produit);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduit", new { produitid = produit.ProduitId }, produit);
    }

    // DELETE: api/Produit/5
    [HttpDelete("{produitid}")]
    public async Task<IActionResult> DeleteProduit(int? produitid)
    {
        var produit = await _context.Produits.FindAsync(produitid);
        if (produit == null)
        {
            return NotFound();
        }

        _context.Produits.Remove(produit);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProduitExists(int? produitid)
    {
        return _context.Produits.Any(e => e.ProduitId == produitid);
    }
}
