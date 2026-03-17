using System;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WorkSpaceApi.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompanyController(ICompanyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var companies = await _service.GetAllAsync();

        if (companies == null)
        {
            return NotFound("Companies not found");
        }

        return Ok(companies);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var company = await _service.GetByIdAsync(id);

        if (company == null)
        {
            return NotFound("Company not found");
        }

        return Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Company request)
    {
        var res = await _service.CreateAsync(request);

        return Ok(res);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, Company request)
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