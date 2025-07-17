using CoreService.Api.Filters;
using CoreService.Core.DTOs.User;
using CoreService.Core.DTOs.UserAccount;
using CoreService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Api.Controllers;

[ApiController]
public class UserController : CustomControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [ValidateModel]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        return await HandleResult(result);
    }

    [HttpPost("login")]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);
        return await HandleResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = UserId!.Value;
        var result = await _userService.GetCurrentUserAsync(userId);
        return await HandleResult(result);
    }

    [HttpPut("me/password")]
    [Authorize]
    [ValidateModel]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = UserId!.Value;
        var result = await _userService.ChangePasswordAsync(userId, request);
        return await HandleResult(result);
    }

    [HttpGet("me/accounts")]
    [Authorize]
    public async Task<IActionResult> GetUserAccounts()
    {
        var userId = UserId!.Value;
        var result = await _userService.GetUserAccountsAsync(userId);
        return await HandleResult(result);
    }

    [HttpPost("me/accounts")]
    [Authorize]
    [ValidateModel]
    public async Task<IActionResult> UpsertUserAccount([FromBody] UserAccountRequest request)
    {
        var userId = UserId!.Value;
        var result = await _userService.UpsertUserAccountAsync(userId, request);
        return await HandleResult(result);
    }

    [HttpDelete("me/accounts/{accountId:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserAccount(long accountId)
    {
        var userId = UserId!.Value;
        var result = await _userService.DeleteUserAccountAsync(userId, accountId);
        return await HandleResult(result);
    }
}