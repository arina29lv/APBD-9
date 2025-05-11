using Microsoft.Data.SqlClient;
using System.Data;

using APBD_09.Infrastructure.DTOs;
using APBD_09.Domain.Models;

namespace APBD_09.Infrastructure.Repositories;

public class WarehouseRepository : IWarehousePerository
{
    private readonly SqlConnection _sqlConnection;

    public WarehouseRepository(SqlConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    // 1
    public async Task<bool> ProductExistsByIdAsync(int productId)
    {
        const string query = "SELECT 1 FROM Product WHERE IdProduct = @IdProduct";
        
        var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@IdProduct", productId);

        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        return result != null;
    }
    
    public async Task<bool> WarehouseExistsByIdAsync(int productId)
    {
        const string query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        
        var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@IdWarehouse", productId);
        
        if(_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }
    
    // 2
    public async Task<Order?> GetMatchingOrderAsync(AddProductToWarehouseDto dto)
    {
        const string query = @"
              SELECT IdOrder, IdProduct, Amount, CreatedAt, FulfilledAt 
              FROM [Order]
              WHERE IdProduct = @IdProduct
              AND Amount = @Amount
              AND CreatedAt < @CreatedAt";
        
        await using var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@IdProduct", dto.IdProduct);
        command.Parameters.AddWithValue("@Amount", dto.Amount);
        command.Parameters.AddWithValue("@CreatedAt", dto.CreatedAt);
        
        if(_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Order
            {
                IdOrder = reader.GetInt32(0),
                IdProduct = reader.GetInt32(1),
                Amount = reader.GetInt32(2),
                CreatedAt = reader.GetDateTime(3),
                FulfilledAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
            };
        }
        return null;
    }
    
    // 3
    public async Task<bool> IsOrderAlreadyExistsByIdAsync(int orderId)
    {
        const string query = @"
              SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        
        var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@IdOrder", orderId);
        
        if(_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }
    
    // 4
    public async Task UpdateOrderFulfilledAtAsync(int orderId)
    {
        const string query =
        @"
               UPDATE [Order] SET FulfilledAt = @Now
               WHERE IdOrder = @IdOrder";
        
        await using var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@Now", DateTime.Now);
        command.Parameters.AddWithValue("@IdOrder", orderId);
        
        if(_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();
        
        await command.ExecuteNonQueryAsync();
    }
    
    // 5
    public async Task<decimal?> GetProductPriceByIdAsync(int productId)
    {
        const string query = @"
              SELECT Price FROM Product 
              WHERE IdProduct = @IdProduct";
        
        await using var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@IdProduct", productId);
        
        if(_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToDecimal(result) : null;
    }

    public async Task<int> InsertProductWarehouseAsync(AddProductToWarehouseDto dto, int orderId,
        decimal calculatedPrice)
    {
        const string query =@"
               INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
               OUTPUT INSERTED.IdProductWarehouse
               VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
        
        await using var command = new SqlCommand(query, _sqlConnection);
        command.Parameters.AddWithValue("@IdWarehouse", dto.IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", dto.IdProduct);
        command.Parameters.AddWithValue("@IdOrder", orderId);
        command.Parameters.AddWithValue("@Amount", dto.Amount);
        command.Parameters.AddWithValue("@Price", calculatedPrice);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        if (_sqlConnection.State != ConnectionState.Open)
            await _sqlConnection.OpenAsync();

        var insertedId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(insertedId);
    }
    
}