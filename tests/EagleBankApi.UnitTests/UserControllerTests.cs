using AutoFixture;
using EagleBankApi.Controllers;
using EagleBankApi.Models;
using EagleBankApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace EagleBankApi.UnitTests;

public class UserControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly UserController _sut;

    public UserControllerTests()
    {
        _sut = new UserController(_userService);
    }

    [Fact]
    public async Task CreateUser_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = _fixture.Build<CreateUserRequest>()
            .With(x=>x.Email, "a@b.com")
            .With(x=> x.PhoneNumber, "+123456789")
            .Create();

        var expectedResponse = _fixture.Create<UserResponse>();

        _userService.CreateUserAsync(Arg.Any<CreateUserRequest>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.CreateUser(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.Value.Should().BeEquivalentTo(expectedResponse);
    }
}
