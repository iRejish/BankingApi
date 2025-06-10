using System.Net;
using System.Net.Http.Json;
using EagleBankApi.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AutoFixture;

namespace EagleBankApi.AcceptanceTests;

public class UserControllerTests(CustomWebApplicationFactory factory) : ControllerTestBase(factory)
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreatedResponse()
    {
        // Arrange
        var request = _fixture.Build<CreateUserRequest>()
            .With(x=>x.Email, "a@b.com")
            .With(x=> x.PhoneNumber, "+123456789")
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync("/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().NotBeNull();
        content.Id.Should().MatchRegex("^usr-[A-Za-z0-9]+$");
        content.Name.Should().Be(request.Name);
        content.Email.Should().Be(request.Email);

        // Verify database state
        var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == content.Id);
        userInDb.Should().NotBeNull();
        userInDb.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task CreateUser_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new CreateUserRequest
        {
            Name = "",
            Address = new Address
            {
                Line1 = "",
                Town = "",
                County = "",
                Postcode = ""
            },
            PhoneNumber = "invalid",
            Email = "not-an-email"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/users", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<BadRequestErrorResponse>();
        errorResponse.Should().NotBeNull();

        errorResponse.Details.Should().Contain(d =>
            d.Field.ToString() == "Name" &&
            d.Message.ToString().Contains("required"));
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange - first create a valid user
        var email = "duplicate@example.com";
        var firstUser = _fixture.Build<CreateUserRequest>()
            .With(x=>x.Email, email)
            .With(x=> x.PhoneNumber, "+123456789")
            .Create();

        await _client.PostAsJsonAsync("/v1/users", firstUser);

        // Act - try to create duplicate
        var secondUser = _fixture.Build<CreateUserRequest>()
            .With(x=>x.Email, email)
            .With(x=> x.PhoneNumber, "+123456789")
            .Create();

        var response = await _client.PostAsJsonAsync("/v1/users", secondUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<BadRequestErrorResponse>();
        error.Message.Should().Contain("Email already in use");
    }
}
