using System;
using Domain.DTOs;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WorkSpaceApi.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly BookingService _service;

    public BookingController(BookingService service)
    {
        _service = service;
    }

    [HttpGet("company/{companyId:int}")]
    public async Task<IActionResult> GetCompanyBookingsAsync(int companyId)
    {
        var res = await _service.GetCompanyBookingsAsync(companyId);

        if(res == null)
        {
            return NotFound("Bookings not found");
        }

        return Ok(res);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetBookingStatisticsAsync()
    {
        var res = await _service.GetBookingStatisticsAsync();

        if(res == null)
        {
            return NotFound("Booking Statistics not found");
        }

        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBookingAsync(CreateBookingRequest request)
    {
        var res = await _service.CreateBookingAsync(request);

        return Ok(res);
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateBookingStatusAsync(int id, string status)
    {
        var res = await _service.UpdateBookingStatusAsync(id, status);

        return Ok(res);
    }

    [HttpGet("daily")]
    public async Task<IActionResult> GetBookingsByDateAsync(DateTime date)
    {
        var res = await _service.GetBookingsByDateAsync(date);

        return Ok(res);
    }
}
