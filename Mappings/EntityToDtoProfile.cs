using AutoMapper;
using InventoryMS.DTOs.Orders;
using InventoryMS.DTOs.Products;
using InventoryMS.DTOs.Suppliers;
using InventoryMS.DTOs.Users;
using InventoryMS.Models;

namespace InventoryMS.Mappings;

public sealed class EntityToDtoProfile : Profile
{
    public EntityToDtoProfile()
    {
        // User → UserResponseDto
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Firstname))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Lastname))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty));

        // Supplier → SupplierResponseDto
        CreateMap<Supplier, SupplierResponseDto>();

        // Product → ProductResponseDto
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDesc))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : string.Empty));

        // OrderItem → OrderItemResponseDto
        CreateMap<OrderItem, OrderItemResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));

        // Order → OrderResponseDto
        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.Firstname} {src.User.Lastname}" : string.Empty));
    }
}
