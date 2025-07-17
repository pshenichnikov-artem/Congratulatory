using CoreService.Core.DTOs.User;
using CoreService.Core.DTOs.UserAccount;
using CoreService.Result;

namespace CoreService.Core.Interfaces;

public interface IUserService
{
    Task<ServiceResult<LoginResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult<UserResponse>> GetCurrentUserAsync(Guid userId);
    Task<ServiceResult<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ServiceResult<IEnumerable<UserAccountResponse>>> GetUserAccountsAsync(Guid userId);
    Task<ServiceResult<UserAccountResponse>> UpsertUserAccountAsync(Guid userId, UserAccountRequest request);
    Task<ServiceResult<bool>> DeleteUserAccountAsync(Guid userId, long accountId);
    Task<ServiceResult<bool>> VerifyUserAccountAsync(string platform, string userName, long chatId);
}