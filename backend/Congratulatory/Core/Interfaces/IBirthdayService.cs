using CoreService.Core.DTOs.Birthday;
using CoreService.Core.DTOs.BirthdayNotification;
using CoreService.Result;

namespace CoreService.Core.Interfaces;

public interface IBirthdayService
{
    Task<ServiceResult<List<BirthdayResponse>>> GetBirthdaysAsync(BirthdaySearchRequest searchRequest, Guid userId);
    Task<ServiceResult<BirthdayResponse>> GetBirthdayByIdAsync(int id, Guid userId);
    Task<ServiceResult<BirthdayResponse>> CreateBirthdayAsync(BirthdayCreateRequest createRequest, Guid userId);
    Task<ServiceResult<BirthdayResponse>> UpdateBirthdayAsync(int id, BirthdayUpdateRequest updateRequest, Guid userId);
    Task<ServiceResult<bool>> DeleteBirthdayAsync(int id, Guid userId);
    Task<ServiceResult<IEnumerable<BirthdayNotificationResponse>>> GetBirthdayNotificationsAsync(long birthdayId, Guid userId);
    Task<ServiceResult<BirthdayNotificationResponse>> UpsertBirthdayNotificationAsync(long birthdayId, BirthdayNotificationRequest request, Guid userId);
    Task<ServiceResult<bool>> DeleteBirthdayNotificationAsync(long birthdayId, long notificationId, Guid userId);
}