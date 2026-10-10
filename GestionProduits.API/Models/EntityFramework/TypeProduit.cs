using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProduits.API.Models.EntityFramework
{
    [Table("t_e_typeproduit_tpt")]
    public class TypeProduit
    {
        [Key]
        [Column("tpt_id")]
        public int TypeProduitId { get; set; }

        [Required(ErrorMessage = "This property must be filled.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The string length must be between 1 and 50 characters.")]
        [Column("tpt_nomtypeproduit")]
        public string NomTypeProduit { get; set; }

        [InverseProperty(nameof(Produit.Type))]
        public ICollection<Produit> Produits { get; set; } = new List<Produit>();
    }
}
