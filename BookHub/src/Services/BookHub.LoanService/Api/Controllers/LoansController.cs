using BookHub.LoanService.Application.Services;
using BookHub.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookHub.LoanService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll(CancellationToken cancellationToken)
    {
        var loans = await _loanService.GetAllLoansAsync(cancellationToken);
        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LoanDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var loan = await _loanService.GetLoanByIdAsync(id, cancellationToken);
        if (loan == null) return NotFound();
        return Ok(loan);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var loans = await _loanService.GetLoansByUserIdAsync(userId, cancellationToken);
        return Ok(loans);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdue(CancellationToken cancellationToken)
    {
        return StatusCode(501, "Not implemented");
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto, CancellationToken cancellationToken)
    {
        return StatusCode(501, "Not implemented");
    }

    [HttpPut("{id:guid}/return")]
    public async Task<ActionResult<LoanDto>> Return(Guid id, CancellationToken cancellationToken)
    {
        return StatusCode(501, "Not implemented");
    }
}
