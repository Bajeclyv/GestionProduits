using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProduits.API.Models.EntityFramework
{
    [Table("t_e_produit_pdt")]
    public class Produit
    {
        [Key]
        [Column("pdt_id")]
        public int ProduitId { get; set; }

        [Column("pdt_nomproduit")]
        public string NomProduit { get; set; }

        [Column("pdt_description")]
        public string Description { get; set; }

        [Column("pdt_nomphoto")]
        public string NomPhoto { get; set; }

        [Column("pdt_uriphoto")]
        public string UriPhoto { get; set; }

        [ForeignKey("TypeProduitId")]
        [InverseProperty(nameof(Marque.Produits))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Marque MarqueProduit { get; set; }

        [ForeignKey("MarqueId")]
        [InverseProperty(nameof(TypeProduit.Produits))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public TypeProduit Type { get; set; }

        [Column("pdt_stockreel")]
        public int StockReel { get; set; }

        [Column("pdt_stockmin")]
        public int StockMin { get; set; }

        [Column("pdt_stockmax")]
        public int StockMax { get; set; }

    }
}
