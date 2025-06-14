using AutoMapper;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.DTO.Article;
using VivesRental.DTO.Auth;
using VivesRental.DTO.Customer;
using VivesRental.DTO.Order;
using VivesRental.DTO.Product;
using VivesRental.DTO.Reservation;
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

        CreateMap<OrderLineCreateDto, OrderLine>();

        CreateMap<Product, ProductDto>();

        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id wordt in controller gegenereerd

        CreateMap<ProductUpdateDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id mag niet overschreven worden

        CreateMap<ArticleReservation, ArticleReservationDto>()
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Article.Product!.Name))
            .ForMember(dest => dest.CustomerFullName, opt =>
                opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"));

        CreateMap<ArticleReservationCreateDto, ArticleReservation>();

        CreateMap<ArticleReservationUpdateDto, ArticleReservation>();

        CreateMap<LoginRequestDto, User>();
    }
}
