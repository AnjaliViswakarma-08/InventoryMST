using AutoMapper;
using InventoryMS.Data;
using InventoryMS.DTOs.Products;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.Data.Models;
using InventoryMS.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<ProductResponseDto>>(products);
    }

    public async Task<ProductResponseDto> GetByIdAsync(int productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithSupplierAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product {productId} was not found.");

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken cancellationToken)
    {
        if (dto.Price <= 0)
        {
            throw new InvalidOperationException("Price must be greater than zero.");
        }

        if (dto.QuantityInStock < 0)
        {
            throw new InvalidOperationException("Quantity cannot be negative.");
        }

        var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId, cancellationToken)
            ?? throw new NotFoundException($"Supplier {dto.SupplierId} was not found.");

        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;
        product.Supplier = supplier;

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(int productId, ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product {productId} was not found.");

        if (dto.Price <= 0)
        {
            throw new InvalidOperationException("Price must be greater than zero.");
        }

        if (dto.QuantityInStock < 0)
        {
            throw new InvalidOperationException("Quantity cannot be negative.");
        }

        var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId, cancellationToken)
            ?? throw new NotFoundException($"Supplier {dto.SupplierId} was not found.");

        _mapper.Map(dto, product);
        product.SupplierId = supplier.SupplierId;
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshed = await _productRepository.GetByIdWithSupplierAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product {productId} was not found.");

        return _mapper.Map<ProductResponseDto>(refreshed);
    }

    public async Task DeleteAsync(int productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product {productId} was not found.");

        _productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
