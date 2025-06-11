using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EagleBankApi.Models;
using EagleBankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EagleBankApi.Controllers;

[ApiController]
[Route("v1/users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var response = await userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUserById), new { userId = response.Id }, response);
    }

    [HttpGet("{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById([FromRoute][RegularExpression(@"^usr-[A-Za-z0-9]+$")] string userId)
    {
        var requestingUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId != requestingUserId) return Forbid();

        var response = await userService.GetUserByIdAsync(userId);
        return Ok(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var response = await userService.LoginUser(request.Email, request.Password);
        return Ok(response);
    }

    [HttpPatch("{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute][RegularExpression(@"^usr-[A-Za-z0-9]+$")] string userId,
        [FromBody] UpdateUserRequest request)
    {
        var requestingUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId != requestingUserId) return Forbid();

        var response = await userService.UpdateUserAsync(userId, request);
        return Ok(response);
    }

    [HttpDelete("{userId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromRoute][RegularExpression(@"^usr-[A-Za-z0-9]+$")] string userId)
    {
        var requestingUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId != requestingUserId) return Forbid();

        await userService.DeleteUserAsync(userId);
        return NoContent();
    }
}
