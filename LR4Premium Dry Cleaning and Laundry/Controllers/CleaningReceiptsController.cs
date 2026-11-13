using DryCleaningApi.DTOs;
using DryCleaningApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaningApi.Controllers;

[ApiController]
[Route("api/cleaning-receipts")]
[Authorize]
public class CleaningReceiptsController : ControllerBase
{
    private readonly ICleaningReceiptService _service;

    public CleaningReceiptsController(ICleaningReceiptService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptOrder([FromBody] AcceptOrderRequestDto dto)
    {
        try
        {
            var result = await _service.AcceptItemsAndCreateReceiptAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpPost("{id}/process")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ProcessCleaning(string id, [FromBody] ProcessCleaningRequestDto dto)
    {
        try
        {
            var result = await _service.ProcessCleaningAndQualityControlAsync(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("issue")]
    public async Task<IActionResult> IssueOrder([FromBody] IssueOrderRequestDto dto)
    {
        try
        {
            var result = await _service.IssueOrderAndHandleCompensationAsync(dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}