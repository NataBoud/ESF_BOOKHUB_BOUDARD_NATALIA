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
        var overdueLoans = await _loanService.GetOverdueLoansAsync(cancellationToken);
        return Ok(overdueLoans);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto, CancellationToken cancellationToken)
    {
        try
        {
            // utilise directement le UserId passé dans le corps de la requête
            var loan = await _loanService.CreateLoanAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPut("{id:guid}/return")]
    public async Task<ActionResult<LoanDto>> Return(Guid id, CancellationToken cancellationToken)
    {
        var loan = await _loanService.ReturnLoanAsync(id, cancellationToken);
        if (loan == null) return NotFound();
        return Ok(loan);
    }

    [HttpGet("book/{bookId:guid}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByBookId(Guid bookId, CancellationToken cancellationToken)
    {
        var loans = await _loanService.GetLoansByBookIdAsync(bookId, cancellationToken);
        return Ok(loans);
    }

    [HttpGet("admin/dashboard")]
    public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _loanService.GetAdminDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    [HttpGet("popular")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetPopularBooks(int limit = 5, CancellationToken cancellationToken = default)
    {
        var popularBooks = await _loanService.GetTopBorrowedBooksAsync(limit, cancellationToken);
        return Ok(popularBooks);
    }

}
