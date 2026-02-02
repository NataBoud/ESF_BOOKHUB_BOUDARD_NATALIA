using BookHub.CatalogService.Application.Services;
using BookHub.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookHub.CatalogService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _bookService.GetPagedBooksAsync(page, pageSize, cancellationToken);

        // Retourner un objet avec les données et le total pour la pagination côté client
        return Ok(new
        {
            Items = items,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetBookByIdAsync(id, cancellationToken);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDto>>> Search([FromQuery] string term, CancellationToken cancellationToken)
    {
        var books = await _bookService.SearchBooksAsync(term, cancellationToken);
        return Ok(books);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetByCategory(string category, CancellationToken cancellationToken)
    {
        var books = await _bookService.GetBooksByCategoryAsync(category, cancellationToken);
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto, CancellationToken cancellationToken)
    {
        var book = await _bookService.CreateBookAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookDto>> Update(Guid id, [FromBody] UpdateBookDto dto, CancellationToken cancellationToken)
    {
        var book = await _bookService.UpdateBookAsync(id, dto, cancellationToken);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookService.DeleteBookAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:guid}/decrement-availability")]
    public async Task<IActionResult> DecrementAvailability(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookService.DecrementAvailabilityAsync(id, cancellationToken);
        if (!result) return BadRequest("Cannot decrement availability");
        return Ok();
    }

    [HttpPost("{id:guid}/increment-availability")]
    public async Task<IActionResult> IncrementAvailability(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookService.IncrementAvailabilityAsync(id, cancellationToken);
        if (!result) return BadRequest("Cannot increment availability");
        return Ok();
    }
}
