using System.ComponentModel.DataAnnotations;
using EagleBankApi.Models;
using EagleBankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EagleBankApi.Controllers;

[ApiController]
[Route("v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var response = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUserById), new { userId = response.Id }, response);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById([FromRoute][RegularExpression(@"^usr-[A-Za-z0-9]+$")] string userId)
    {
        var response = await _userService.GetUserByIdAsync(userId);
        return Ok(response);
    }
}
