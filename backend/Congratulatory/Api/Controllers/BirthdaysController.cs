using CoreService.Core.DTOs.Birthday;
using CoreService.Core.DTOs.BirthdayNotification;
using CoreService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CoreService.Result;
using CoreService.Api.Filters;

namespace CoreService.Api.Controllers;

[ApiController]
public class BirthdaysController : CustomControllerBase
{
    private readonly IBirthdayService _birthdayService;

    public BirthdaysController(IBirthdayService birthdayService)
    {
        _birthdayService = birthdayService;
    }

    [HttpPost("search")]
    [ValidateModel]
    [ValidateToken]
    public async Task<IActionResult> SearchBirthdays([FromBody] BirthdaySearchRequest request)
    {
        var userId = UserId!.Value;
        var result = await _birthdayService.GetBirthdaysAsync(request, userId);
        return await HandleResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBirthday(int id)
    {
        var userId = UserId!.Value;
        var result = await _birthdayService.GetBirthdayByIdAsync(id, userId);
        return await HandleResult(result);
    }

    [HttpPost]
    [ValidateModel]
    [ValidateToken]
    public async Task<IActionResult> CreateBirthday([FromForm] BirthdayCreateRequest request)
    {
        var userId = UserId.Value;
        var result = await _birthdayService.CreateBirthdayAsync(request, userId);
        
        if (result.Success)
        {
            return CreatedAtAction(
                nameof(GetBirthday),
                new { id = result.Data!.Id },
                ApiResult.Ok(result.Data)
            );
        }
        
        return await HandleResult(result);
    }

    [HttpPut("{id:int}")]
    [ValidateModel]
    public async Task<IActionResult> UpdateBirthday(int id, [FromForm] BirthdayUpdateRequest request)
    {
        var userId = UserId!.Value;
        var result = await _birthdayService.UpdateBirthdayAsync(id, request, userId);
        return await HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBirthday(int id)
    {
        var userId = UserId!.Value;
        var result = await _birthdayService.DeleteBirthdayAsync(id, userId);
        return await HandleResult(result);
    }

    [HttpGet("{birthdayId:long}/notifications")]
    public async Task<IActionResult> GetBirthdayNotifications(long birthdayId)
    {
        var userId = UserId!.Value;
        var result = await _birthdayService.GetBirthdayNotificationsAsync(birthdayId, userId);
        return await HandleResult(result);
    }

    [HttpPost("{birthdayId:long}/notifications")]
    [ValidateModel]
    public async Task<IActionResult> UpsertBirthdayNotification(long birthdayId, [FromBody] BirthdayNotificationRequest request)
    {
        var userId = UserId!.Value;
        var result = await _birthdayService.UpsertBirthdayNotificationAsync(birthdayId, request, userId);
        return await HandleResult(result);
    }

    [HttpDelete("{birthdayId:long}/notifications/{notificationId:long}")]
    public async Task<IActionResult> DeleteBirthdayNotification(long birthdayId, long notificationId)
    {
        var userId = UserId!.Value;
        var result = await _birthdayService.DeleteBirthdayNotificationAsync(birthdayId, notificationId, userId);
        return await HandleResult(result);
    }
}