using System;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WorkSpaceApi.Controllers;

[ApiController]
[Route("api/workspaces")]
public class WorkSpaceController : ControllerBase
{
    private readonly IWorkSpaceService _service;

    public WorkSpaceController(IWorkSpaceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var workSpaces = await _service.GetAllAsync();

        if (workSpaces == null)
        {
            return NotFound("WorkSpaces not found");
        }

        return Ok(workSpaces);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var workSpace = await _service.GetByIdAsync(id);

        if (workSpace == null)
        {
            return NotFound("WorkSpace not found");
        }

        return Ok(workSpace);
    }

    [HttpGet("room/{roomId:int}")]
    public async Task<IActionResult> GetByRoomAsync(int roomId)
    {
        var res = await _service.GetByRoomAsync(roomId);

        if(res == null)
        {
            return NotFound("WorkSpaces not found");
        }

        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(WorkSpace request)
    {
        var res = await _service.CreateAsync(request);

        return Ok(res);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, WorkSpace request)
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
