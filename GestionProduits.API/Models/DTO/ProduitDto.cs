namespace GestionProduits.API.Models.DTO
{
    public class ProduitDto : IEntityDto
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Type { get; set; }
        public string? Marque { get; set; }
    }
}
