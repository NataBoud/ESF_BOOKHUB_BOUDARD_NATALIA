using BookHub.Shared.DTOs;
using BookHub.UserService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookHub.UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.LoginAsync(dto, cancellationToken);
        if (result == null) return Unauthorized(new { message = "Invalid email or password" });
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateUserAsync(id, dto, cancellationToken);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUserAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
