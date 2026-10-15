namespace GestionProduits.Application.Models;

public sealed class CatalogItemDto
{
    public int Id { get; set; }
    public string? Nom { get; set; }
    public int NbProduits { get; set; }
}
