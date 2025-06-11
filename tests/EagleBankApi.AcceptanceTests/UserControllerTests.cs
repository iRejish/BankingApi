using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EagleBankApi.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBankApi.AcceptanceTests;

public class UserControllerTests(CustomWebApplicationFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreatedResponse()
    {
        // Arrange
        var request = _fixture.Build<CreateUserRequest>()
            .With(x => x.Email, "a@b.com")
            .With(x => x.PhoneNumber, "+123456789")
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
            d.Field == "Name" &&
            d.Message.Contains("required"));
    }

    [Fact]
    public async Task GetUserById_WithValidToken_ReturnsUser()
    {
        // Arrange
        var user = await CreateTestUser();
        AuthenticateClient(user.Id);

        // Act
        var response = await _client.GetAsync($"/v1/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserById_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var user = await CreateTestUser();

        // Act (no token set)
        var response = await _client.GetAsync($"/v1/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUser_UpdatesUser_WithValidData()
    {
        // Arrange
        var user = await CreateTestUser();
        AuthenticateClient(user.Id);

        var updateRequest = new UpdateUserRequest
        {
            Name = "Updated Name",
            PhoneNumber = "+449876543210"
        };

        // Act
        var response = await _client.PatchAsJsonAsync($"/v1/users/{user.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.Equal("Updated Name", updatedUser.Name);
        Assert.Equal("+449876543210", updatedUser.PhoneNumber);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var user = await CreateTestUser();
        AuthenticateClient(user.Id);

        // Act
        var response = await _client.DeleteAsync($"/v1/users/{user.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
