using System.ComponentModel.DataAnnotations;

namespace GestionProduits.API.Models.DTO;

public class TypeProduitDto : IEntityDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string? Nom { get; set; }

    public int NbProduits { get; set; }
}
