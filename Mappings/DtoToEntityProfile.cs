using AutoMapper;
using InventoryMS.DTOs.Products;
using InventoryMS.DTOs.Suppliers;
using InventoryMS.DTOs.Users;
using InventoryMS.Data.Models;

namespace InventoryMS.Mappings;

public sealed class DtoToEntityProfile : Profile
{
    public DtoToEntityProfile()
    {
        // UserCreateDto → User
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // UserUpdateDto → User
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // SupplierCreateDto → Supplier
        CreateMap<SupplierCreateDto, Supplier>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierId, opt => opt.Ignore());

        // SupplierUpdateDto → Supplier
        CreateMap<SupplierUpdateDto, Supplier>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierId, opt => opt.Ignore());

        // ProductCreateDto → Product
        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.ProductDesc, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // ProductUpdateDto → Product
        CreateMap<ProductUpdateDto, Product>()
            .ForMember(dest => dest.ProductDesc, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
