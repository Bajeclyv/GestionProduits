using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProduits.API.Models.EntityFramework
{
    [Table("t_e_marque_mqe")]
    public class Marque
    {
        [Key]
        [Column("mqe_id")]
        public int MarqueId { get; set; }

        [Column("mqe_nommarque")]
        public string NomMarque { get; set; }

        [InverseProperty(nameof(Produit.MarqueProduit))]
        public ICollection<Produit> Produits { get; set; }
    }
}
