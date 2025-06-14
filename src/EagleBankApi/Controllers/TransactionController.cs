using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EagleBankApi.Application.Models;
using EagleBankApi.Application.Services;
using EagleBankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EagleBankApi.Controllers;

[ApiController]
[Route("v1/accounts/{accountNumber}/transactions")]
[Authorize]
public class TransactionController(ITransactionService transactionService, IAccountService accountService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTransaction(
        [FromRoute][RegularExpression(@"^01\d{6}$")] string accountNumber,
        [FromBody] CreateTransactionRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var response = await transactionService.CreateTransactionAsync(userId, accountNumber, request);

        return CreatedAtAction(
            nameof(GetTransaction),
            new { accountNumber, transactionId = response.Id },
            response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListTransactionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListTransactions(
        [FromRoute][RegularExpression(@"^01\d{6}$")] string accountNumber)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var response = await transactionService.ListTransactionsAsync(userId, accountNumber);
        return Ok(response);
    }

    [HttpGet("{transactionId}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTransaction(
        [FromRoute][RegularExpression(@"^01\d{6}$")] string accountNumber,
        [FromRoute][RegularExpression(@"^tan-[A-Za-z0-9]+$")] string transactionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var response = await transactionService.GetTransactionAsync(userId, accountNumber, transactionId);
        return Ok(response);
    }
}
