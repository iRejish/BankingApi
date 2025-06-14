using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EagleBankApi.Application.Models;
using EagleBankApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EagleBankApi.Controllers;

[ApiController]
[Route("v1/accounts")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateBankAccountRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _accountService.CreateAccountAsync(request, userId);
        return CreatedAtAction(nameof(GetAccountByNumber), new { accountNumber = response.AccountNumber }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListBankAccountsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListAccounts()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _accountService.ListAccountsAsync(userId);
        return Ok(response);
    }

    [HttpGet("{accountNumber}")]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAccountByNumber([FromRoute][RegularExpression(@"^01\d{6}$")] string accountNumber)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _accountService.GetAccountByNumberAsync(accountNumber, userId);
        return Ok(response);
    }

    [HttpPatch("{accountNumber}")]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAccount(
        [FromRoute][RegularExpression(@"^01\d{6}$")] string accountNumber,
        [FromBody] UpdateBankAccountRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _accountService.UpdateAccountAsync(accountNumber, request, userId);
        return Ok(response);
    }

    [HttpDelete("{accountNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAccount([FromRoute][RegularExpression(@"^01\d{6}$")] string accountNumber)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _accountService.DeleteAccountAsync(accountNumber, userId);
        return NoContent();
    }
}
