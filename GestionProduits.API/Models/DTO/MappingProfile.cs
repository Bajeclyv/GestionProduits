using AutoMapper;
using GestionProduits.API.Models.EntityFramework;

namespace GestionProduits.API.Models.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produit, ProduitDto>(MemberList.Destination)
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(src => src.Type.NomTypeProduit))
                .ForMember(
                    dest => dest.Marque,
                    opt => opt.MapFrom(src => src.MarqueProduit.NomMarque));

            CreateMap<Produit, ProduitDetailDto>()
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(src => src.Type.NomTypeProduit))
                .ForMember(
                    dest => dest.Marque,
                    opt => opt.MapFrom(src => src.MarqueProduit.NomMarque))
                .ForMember(
                    dest => dest.EnReappro,
                    opt => opt.MapFrom(src => src.StockReel < src.StockMin));

            CreateMap<ProduitDto, Produit>();

            CreateMap<Marque, MarqueDto>()
                .ForMember(
                    dest => dest.NbProduits,
                    opt => opt.MapFrom(src => src.Produits.Count()));
        }
    }
}