using AutoMapper;
using InventoryMS.Data;
using InventoryMS.DTOs.Suppliers;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using InventoryMS.Repositories.Interfaces;
using InventoryMS.Exceptions;

namespace InventoryMS.Services;

public sealed class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _dbContext;

    public SupplierService(ISupplierRepository supplierRepository, IMapper mapper, AppDbContext dbContext)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<List<SupplierResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<SupplierResponseDto>>(suppliers);
    }

    public async Task<SupplierResponseDto> GetByIdAsync(int supplierId, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId, cancellationToken)
            ?? throw new NotFoundException($"Supplier {supplierId} was not found.");

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto, CancellationToken cancellationToken)
    {
        if (await _supplierRepository.EmailExistsAsync(dto.Email, null, cancellationToken))
        {
            throw new ConflictException("Supplier email already exists.");
        }

        var supplier = _mapper.Map<InventoryMS.Models.Supplier>(dto);
        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<SupplierResponseDto> UpdateAsync(int supplierId, SupplierUpdateDto dto, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId, cancellationToken)
            ?? throw new NotFoundException($"Supplier {supplierId} was not found.");

        if (await _supplierRepository.EmailExistsAsync(dto.Email, supplierId, cancellationToken))
        {
            throw new ConflictException("Supplier email already exists.");
        }

        _mapper.Map(dto, supplier);
        _supplierRepository.Update(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task DeleteAsync(int supplierId, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId, cancellationToken)
            ?? throw new NotFoundException($"Supplier {supplierId} was not found.");

        _supplierRepository.Remove(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
