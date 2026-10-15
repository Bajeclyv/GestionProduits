using System.ComponentModel.DataAnnotations;

namespace GestionProduits.API.Models.DTO
{
    public class MarqueDto : IEntityDto
    {
        public int Id { get; set; }

        [Required]
        public string? Nom { get; set; }

        public int NbProduits { get; set; }
    }
}
