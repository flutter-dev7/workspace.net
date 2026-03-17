using System;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WorkSpaceApi.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _service;

    public RoomController(IRoomService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var rooms = await _service.GetAllAsync();

        if(rooms == null)
        {
            return NotFound("Rooms not found");
        }

        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var room = await _service.GetByIdAsync(id);

        if(room == null)
        {
            return NotFound("Room not found");
        }

        return Ok(room);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Room request)
    {
        var res = await _service.CreateAsync(request);

        return Ok(res);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, Room request)
    {
        var res = await _service.UpdateAsync(id, request);

        return Ok(res);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var res = await _service.DeleteAsync(id);

        return Ok(res);
    }
}
