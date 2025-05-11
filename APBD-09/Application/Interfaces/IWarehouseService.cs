using APBD_09.Infrastructure.DTOs;

namespace APBD_09.Application.Interfaces;

public interface IWarehouseService
{
    Task<(int result, string message)> AddProductToWarehouseAsync(AddProductToWarehouseDto dto);
}