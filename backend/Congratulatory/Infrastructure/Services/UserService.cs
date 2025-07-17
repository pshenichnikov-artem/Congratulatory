using AutoMapper;
using CoreService.Core.DTOs.User;
using CoreService.Core.DTOs.UserAccount;
using CoreService.Core.Entities;
using CoreService.Core.Interfaces;
using CoreService.Infrastructure.Data;
using CoreService.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public UserService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ServiceResult<LoginResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return ServiceResult<LoginResponse>.Conflict("Пользователь с таким email уже существует");
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(
                e => e.Code.ToLower(),
                e => new[] { e.Description }
            );
            return ServiceResult<LoginResponse>.BadRequest("Ошибка при создании пользователя", errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        var userResponse = _mapper.Map<UserResponse>(user);

        var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email!, "User");

        var loginResponse = new LoginResponse
        {
            Token = token,
            User = userResponse
        };

        return ServiceResult<LoginResponse>.Ok(loginResponse);
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return ServiceResult<LoginResponse>.BadRequest("Неверный email или пароль");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return ServiceResult<LoginResponse>.BadRequest("Неверный email или пароль");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email!, role);

        var userResponse = new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        var loginResponse = new LoginResponse
        {
            Token = token,
            User = userResponse
        };

        return ServiceResult<LoginResponse>.Ok(loginResponse);
    }

    public async Task<ServiceResult<UserResponse>> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userManager.Users
            .Include(u => u.UserAccounts)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return ServiceResult<UserResponse>.NotFound("Пользователь не найден");
        }

        var userResponse = _mapper.Map<UserResponse>(user);

        return ServiceResult<UserResponse>.Ok(userResponse);
    }

    public async Task<ServiceResult<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ServiceResult<bool>.NotFound("Пользователь не найден");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(
                e => e.Code.ToLower(),
                e => new[] { e.Description }
            );
            return ServiceResult<bool>.BadRequest("Неверный пароль", errors);
        }

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<IEnumerable<UserAccountResponse>>> GetUserAccountsAsync(Guid userId)
    {
        try
        {
            var accounts = await _context.UserAccounts
                .Where(ua => ua.UserId == userId)
                .ToListAsync();

            var response = _mapper.Map<List<UserAccountResponse>>(accounts);
            return ServiceResult<IEnumerable<UserAccountResponse>>.Ok(response);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<UserAccountResponse>>.Fail(500, $"Ошибка при получении аккаунтов: {ex.Message}");
        }
    }

    public async Task<ServiceResult<UserAccountResponse>> UpsertUserAccountAsync(Guid userId, UserAccountRequest request)
    {
        try
        {
            var existingAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.Platform == request.Platform);

            var existingAccountWithUserName = await _context.UserAccounts
                .FirstOrDefaultAsync(ua => ua.Platform == request.Platform && ua.UserName == request.UserName);

            if(existingAccountWithUserName != null && existingAccountWithUserName.UserId != userId)
                return ServiceResult<UserAccountResponse>.Conflict("Аккаунт с таким именем уже существует");

            if (existingAccount != null)
            {
                if (existingAccount.UserName != request.UserName)
                {
                    existingAccount.IsVerified = false;
                }

                _mapper.Map(request, existingAccount);
                existingAccount.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var updatedResponse = _mapper.Map<UserAccountResponse>(existingAccount);
                return ServiceResult<UserAccountResponse>.Ok(updatedResponse);
            }

            var newAccount = _mapper.Map<UserAccount>(request);
            newAccount.UserId = userId;
            newAccount.CreatedAt = DateTime.UtcNow;
            newAccount.UpdatedAt = DateTime.UtcNow;

            _context.UserAccounts.Add(newAccount);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<UserAccountResponse>(newAccount);
            return ServiceResult<UserAccountResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            return ServiceResult<UserAccountResponse>.Fail(500, $"Ошибка при сохранении аккаунта: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteUserAccountAsync(Guid userId, long accountId)
    {
        try
        {
            var account = await _context.UserAccounts
                .FirstOrDefaultAsync(ua => ua.Id == accountId && ua.UserId == userId);

            if (account == null)
                return ServiceResult<bool>.NotFound("Аккаунт не найден");

            _context.UserAccounts.Remove(account);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(500, $"Ошибка при удалении аккаунта: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> VerifyUserAccountAsync(string platform, string userName, long chatId)
    {
        try
        {
            var account = await _context.UserAccounts
                .FirstOrDefaultAsync(ua => ua.Platform == platform && ua.UserName == userName);

            if (account == null)
                return ServiceResult<bool>.NotFound("Аккаунт не найден");

            account.ChatId = chatId;
            account.IsVerified = true;
            account.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(500, $"Ошибка при верификации аккаунта: {ex.Message}");
        }
    }
}