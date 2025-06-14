using AutoMapper;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.DTO.Article;
using VivesRental.DTO.Customer;
using VivesRental.DTO.Order;
using VivesRental.Repositories;

namespace VivesRental.Mapping;

public class VivesRentalProfile : Profile
{
    public VivesRentalProfile()
    {
        // Entity → DTO
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        // DTO → Entity
        CreateMap<ArticleCreateDto, Article>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ArticleStatus)src.Status));

        CreateMap<ArticleUpdateDto, Article>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ArticleStatus)src.Status));

        CreateMap<ArticleBulkCreateDto, Article>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ArticleStatus)src.Status));

        CreateMap<Customer, CustomerDto>();

        CreateMap<CustomerCreateDto, Customer>();

        CreateMap<CustomerUpdateDto, Customer>();

        CreateMap<Order, OrderDto>();

        CreateMap<OrderCreateDto, Order>();

        CreateMap<OrderLine, OrderLineDto>();

        // Als je OrderLineCreateDto wil mappen (optioneel)
        CreateMap<OrderLineCreateDto, OrderLine>();

    }
}
