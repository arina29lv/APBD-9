using APBD_09.Infrastructure.DTOs;
using APBD_09.Domain.Models;

namespace APBD_09.Infrastructure.Repositories;

public interface IWarehousePerository
{
    
    // 1
    Task<bool> ProductExistsByIdAsync(int productId);
    Task<bool> WarehouseExistsByIdAsync(int productId);
    
    // 2 
    Task<Order?> GetMatchingOrderAsync(AddProductToWarehouseDto dto);
    
    // 3
    Task<bool> IsOrderAlreadyExistsByIdAsync(int orderId);
    
    //4
    Task UpdateOrderFulfilledAtAsync(int orderId);
    
    // 5
    Task<decimal?> GetProductPriceByIdAsync(int productId);
    Task<int> InsertProductWarehouseAsync(AddProductToWarehouseDto dto, int orderId, decimal calculatedPrice);
    
}