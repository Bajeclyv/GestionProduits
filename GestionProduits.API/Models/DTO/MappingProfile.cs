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
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.ProduitId))
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(
                        src => src.Type == null ? null : src.Type.NomTypeProduit))
                .ForMember(
                    dest => dest.Marque,
                    opt => opt.MapFrom(
                        src => src.MarqueProduit == null
                            ? null
                            : src.MarqueProduit.NomMarque));

            CreateMap<Produit, ProduitDetailDto>(MemberList.Destination)
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.ProduitId))
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(
                        src => src.Type == null ? null : src.Type.NomTypeProduit))
                .ForMember(
                    dest => dest.Marque,
                    opt => opt.MapFrom(
                        src => src.MarqueProduit == null
                            ? null
                            : src.MarqueProduit.NomMarque))
                .ForMember(
                    dest => dest.Nomphoto,
                    opt => opt.MapFrom(src => src.NomPhoto))
                .ForMember(
                    dest => dest.Uriphoto,
                    opt => opt.MapFrom(src => src.UriPhoto))
                .ForMember(
                    dest => dest.EnReappro,
                    opt => opt.MapFrom(src => src.Stock < src.StockMin));

            CreateMap<ProduitDto, Produit>(MemberList.Destination)
                .ForMember(
                    dest => dest.ProduitId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.MarqueProduit, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.NomPhoto, opt => opt.Ignore())
                .ForMember(dest => dest.UriPhoto, opt => opt.Ignore())
                .ForMember(dest => dest.Stock, opt => opt.Ignore())
                .ForMember(dest => dest.StockMin, opt => opt.Ignore())
                .ForMember(dest => dest.StockMax, opt => opt.Ignore());

            CreateMap<Marque, MarqueDto>(MemberList.Destination)
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.MarqueId))
                .ForMember(
                    dest => dest.Nom,
                    opt => opt.MapFrom(src => src.NomMarque))
                .ForMember(
                    dest => dest.NbProduits,
                    opt => opt.MapFrom(src => src.Produits.Count()));
        }
    }
}