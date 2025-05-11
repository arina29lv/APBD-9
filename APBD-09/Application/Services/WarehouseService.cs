using System.Data;
using APBD_09.Application.Interfaces;
using APBD_09.Infrastructure.DTOs;
using APBD_09.Domain.Models;
using APBD_09.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;

namespace APBD_09.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehousePerository _warehousePerository;
    private readonly SqlConnection _sqlConnection;

    public WarehouseService(IWarehousePerository warehousePerository, SqlConnection sqlConnection)
    {
        _warehousePerository = warehousePerository;
        _sqlConnection = sqlConnection;
    }

    public async Task<(int result, string message)> AddProductToWarehouseAsync(AddProductToWarehouseDto dto)
    {
        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await using var transaction = await _sqlConnection.BeginTransactionAsync();

        try
        {
            // 1 
            if (dto.Amount < 0)
                return (-1, "Amount must be greater than 0");

            var productExists = await _warehousePerository.ProductExistsByIdAsync(dto.IdProduct);
            if(!productExists)
                return (-1, "Product does not exist");
        
            var warehouseExists = await _warehousePerository.WarehouseExistsByIdAsync(dto.IdWarehouse);
            if(!warehouseExists)
                return (-1, "Warehouse does not exist");
        
            // 2 
            var order =await _warehousePerository.GetMatchingOrderAsync(dto);
            if(order == null)
                return (-1, "Order does not exist");
        
            // 3
            var orderAlreadyExist = await _warehousePerository.IsOrderAlreadyExistsByIdAsync(order.IdOrder);
            if(orderAlreadyExist)
                return (-1, "Order already exist");
        
            // 4
            await _warehousePerository.UpdateOrderFulfilledAtAsync(order.IdOrder);
        
            // 5 
            var price = await _warehousePerository.GetProductPriceByIdAsync(dto.IdProduct);
            if (price == null)
                return (-1, "Price does not exists");
        
            var totalPrice = price.Value * dto.Amount;
        
            var insertedId = await _warehousePerository.InsertProductWarehouseAsync(dto, order.IdOrder, totalPrice);
            // 6 
            return (insertedId, "Item successfully added");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (-1, ex.Message);
        }
    }
}