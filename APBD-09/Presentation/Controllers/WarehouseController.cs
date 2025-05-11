using APBD_09.Application.Interfaces;
using APBD_09.Infrastructure.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APBD_09.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] AddProductToWarehouseDto dto)
    {
        var res = await _warehouseService.AddProductToWarehouseAsync(dto);
        return res.result != -1
            ? Ok(new { Id = res.result, Message = res.message })
            : BadRequest(new { Message = res.message });
    }
}