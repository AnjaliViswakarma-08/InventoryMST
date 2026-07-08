using AutoMapper;
using InventoryMS.DTOs.Orders;
using InventoryMS.DTOs.Products;
using InventoryMS.DTOs.Suppliers;
using InventoryMS.DTOs.Users;
using InventoryMS.Models;

namespace InventoryMS.Mappings;

public sealed class InventoryProfile : Profile
{
    public InventoryProfile()
    {
        // User mapping
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Firstname))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Lastname))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt));
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Supplier mapping
        CreateMap<Supplier, SupplierResponseDto>();
        CreateMap<SupplierCreateDto, Supplier>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierId, opt => opt.Ignore());
        CreateMap<SupplierUpdateDto, Supplier>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierId, opt => opt.Ignore());

        // Product mapping
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDesc))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : string.Empty));
        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.ProductDesc, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<ProductUpdateDto, Product>()
            .ForMember(dest => dest.ProductDesc, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Order item mapping
        CreateMap<OrderItem, OrderItemResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));

        // Order mapping
        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.Firstname} {src.User.Lastname}" : string.Empty));
    }
}
