using AutoMapper;
using CoreService.Core.DTOs.Birthday;
using CoreService.Core.DTOs.BirthdayNotification;
using CoreService.Core.Entities;
using CoreService.Core.Interfaces;
using CoreService.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CoreService.Result;
using System.Globalization;
using CoreService.Common;

namespace CoreService.Infrastructure.Services;

public class BirthdayService : IBirthdayService
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BirthdayService(IMapper mapper, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResult<List<BirthdayResponse>>> GetBirthdaysAsync(BirthdaySearchRequest searchRequest, Guid userId)
    {
        try
        {
            var query = _context.Birthdays
                .Include(b => b.BirthdayNotifications)
                .ThenInclude(bn => bn.UserAccount)
                .Where(b => b.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchRequest.Filter.Name))
            {
                query = query.Where(b => b.FullName.ToLower().Contains(searchRequest.Filter.Name.ToLower()));
            }

            if (searchRequest.Filter.Month.HasValue)
            {
                query = query.Where(b => b.DateOfBirth.Month == searchRequest.Filter.Month);
            }

            if (searchRequest.Filter.RelationshipType != null)
            {
                query = query.Where(b => b.RelationshipType == searchRequest.Filter.RelationshipType);
            }

            if (searchRequest.Filter.UpcomingDays.HasValue)
            {
                var today = DateTime.Today;
                var endDate = today.AddDays(searchRequest.Filter.UpcomingDays!.Value);

                var todayMonth = today.Month;
                var todayDay = today.Day;

                var endMonth = endDate.Month;
                var endDay = endDate.Day;

                query = query.Where(b =>
                    (b.DateOfBirth.Month > todayMonth || (b.DateOfBirth.Month == todayMonth && b.DateOfBirth.Day >= todayDay)) &&
                    (b.DateOfBirth.Month < endMonth || (b.DateOfBirth.Month == endMonth && b.DateOfBirth.Day <= endDay))
                );
            }

            var totalCount = query.Count();

            if (!string.IsNullOrWhiteSpace(searchRequest.Sort.SortBy))
            {
                query = searchRequest.Sort.SortBy.ToLower() switch
                {
                    "name" => searchRequest.Sort.Direction == SortDirection.Asc
                        ? query.OrderBy(b => b.FullName)
                        : query.OrderByDescending(b => b.FullName),
                    "dateofbirth" => searchRequest.Sort.Direction == SortDirection.Asc
                        ? query.OrderBy(b => b.DateOfBirth.Month).ThenBy(b => b.DateOfBirth.Day)
                        : query.OrderByDescending(b => b.DateOfBirth.Month).ThenByDescending(b => b.DateOfBirth.Day),
                    "upcomingdays" when searchRequest.Filter.UpcomingDays.HasValue => searchRequest.Sort.Direction == SortDirection.Asc
                        ? query.OrderBy(b =>
                            (b.DateOfBirth.Month * 100 + b.DateOfBirth.Day) < (DateTime.Today.Month * 100 + DateTime.Today.Day)
                                ? (b.DateOfBirth.Month * 100 + b.DateOfBirth.Day) + 1200
                                : (b.DateOfBirth.Month * 100 + b.DateOfBirth.Day))
                        : query.OrderByDescending(b =>
                            (b.DateOfBirth.Month * 100 + b.DateOfBirth.Day) < (DateTime.Today.Month * 100 + DateTime.Today.Day)
                                ? (b.DateOfBirth.Month * 100 + b.DateOfBirth.Day) + 1200
                                : (b.DateOfBirth.Month * 100 + b.DateOfBirth.Day)),
                    _ => query.OrderBy(b => b.FullName)
                };
            }
            else
            {
                query = query.OrderBy(b => b.FullName);
            }

            var items = await query.ToListAsync();
            var itemsResponse = _mapper.Map<List<BirthdayResponse>>(items);

            return ServiceResult<List<BirthdayResponse>>.Ok(itemsResponse);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<BirthdayResponse>>.Fail(500, $"Ошибка при получении списка дней рождения: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BirthdayResponse>> GetBirthdayByIdAsync(int id, Guid userId)
    {
        try
        {
            var birthday = await _context.Birthdays.FirstOrDefaultAsync(b => b.Id == id);
            if (birthday == null)
                return ServiceResult<BirthdayResponse>.NotFound("День рождения не найден");

            if (birthday.UserId != userId)
                return ServiceResult<BirthdayResponse>.BadRequest("Доступ запрещен");

            return ServiceResult<BirthdayResponse>.Ok(_mapper.Map<BirthdayResponse>(birthday));
        }
        catch (Exception ex)
        {
            return ServiceResult<BirthdayResponse>.Fail(500, $"Ошибка при получении дня рождения: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BirthdayResponse>> CreateBirthdayAsync(BirthdayCreateRequest createRequest, Guid userId)
    {
        try
        {
            string? photoPath = null;
            if (createRequest.Photo != null)
            {
                photoPath = await SavePhotoAsync(createRequest.Photo);
            }

            var birthday = _mapper.Map<Birthday>(createRequest);
            birthday.UserId = userId;
            birthday.PhotoPath = photoPath;

            await _context.Birthdays.AddAsync(birthday);
            await _context.SaveChangesAsync();

            return ServiceResult<BirthdayResponse>.Ok(_mapper.Map<BirthdayResponse>(birthday));
        }
        catch (Exception ex)
        {
            return ServiceResult<BirthdayResponse>.Fail(500, $"Ошибка при создании дня рождения: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BirthdayResponse>> UpdateBirthdayAsync(int id, BirthdayUpdateRequest updateRequest, Guid userId)
    {
        try
        {
            var birthday = await _context.Birthdays.FirstOrDefaultAsync(b => b.Id == id);
            if (birthday == null)
                return ServiceResult<BirthdayResponse>.NotFound("День рождения не найден");

            if (birthday.UserId != userId)
                return ServiceResult<BirthdayResponse>.BadRequest("Доступ запрещен");

            if (!string.IsNullOrEmpty(birthday.PhotoPath))
            {
                await DeletePhotoAsync(birthday.PhotoPath);
                birthday.PhotoPath = null;
            }

            if (updateRequest.Photo != null)
            {
                birthday.PhotoPath = await SavePhotoAsync(updateRequest.Photo);
            }

            birthday.FullName = updateRequest.FullName;
            birthday.DateOfBirth = updateRequest.DateOfBirth;

            _context.Birthdays.Update(birthday);
            await _context.SaveChangesAsync();

            var updatedBirthday = _mapper.Map<BirthdayResponse>(birthday);
            return ServiceResult<BirthdayResponse>.Ok(updatedBirthday);
        }
        catch (Exception ex)
        {
            return ServiceResult<BirthdayResponse>.Fail(500, $"Ошибка при обновлении дня рождения: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteBirthdayAsync(int id, Guid userId)
    {
        try
        {
            var birthday = await _context.Birthdays.FirstOrDefaultAsync(b => b.Id == id);
            if (birthday == null)
                return ServiceResult<bool>.NotFound("День рождения не найден");

            if (birthday.UserId != userId)
                return ServiceResult<bool>.BadRequest("Доступ запрещен");

            if (birthday == null)
                return ServiceResult<bool>.NotFound("День рождения не найден");

            if (!string.IsNullOrEmpty(birthday.PhotoPath))
            {
                await DeletePhotoAsync(birthday.PhotoPath);
            }

            _context.Birthdays.Remove(birthday);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(500, $"Ошибка при удалении дня рождения: {ex.Message}");
        }
    }



    private async Task<string> SavePhotoAsync(IFormFile photo)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            throw new Exception("Не удалось получить адрес сервера");
        var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : "";

        var extension = Path.GetExtension(photo.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "birthdays");

        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }

        var filePath = Path.Combine(uploadsDir, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        return $"{baseUrl}/uploads/birthdays/{fileName}";
    }

    private async Task DeletePhotoAsync(string photoPath)
    {
        if (string.IsNullOrEmpty(photoPath))
            return;

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoPath.TrimStart('/'));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        await Task.CompletedTask;
    }

    public async Task<ServiceResult<IEnumerable<BirthdayNotificationResponse>>> GetBirthdayNotificationsAsync(long birthdayId, Guid userId)
    {
        try
        {
            var birthday = await _context.Birthdays.FirstOrDefaultAsync(b => b.Id == birthdayId && b.UserId == userId);
            if (birthday == null)
                return ServiceResult<IEnumerable<BirthdayNotificationResponse>>.NotFound("День рождения не найден");

            var notifications = await _context.BirthdayNotifications
                .Include(bn => bn.UserAccount)
                .Where(bn => bn.BirthdayId == birthdayId)
                .ToListAsync();

            var response = _mapper.Map<List<BirthdayNotificationResponse>>(notifications);
            return ServiceResult<IEnumerable<BirthdayNotificationResponse>>.Ok(response);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<BirthdayNotificationResponse>>.Fail(500, $"Ошибка при получении уведомлений: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BirthdayNotificationResponse>> UpsertBirthdayNotificationAsync(long birthdayId, BirthdayNotificationRequest request, Guid userId)
    {
        try
        {
            var birthday = await _context.Birthdays.FirstOrDefaultAsync(b => b.Id == birthdayId && b.UserId == userId);
            if (birthday == null)
                return ServiceResult<BirthdayNotificationResponse>.NotFound("День рождения не найден");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ServiceResult<BirthdayNotificationResponse>.NotFound("Аккаунт не найден");

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Id == request.UserAccountId);
            if (userAccount == null)
                return ServiceResult<BirthdayNotificationResponse>.NotFound("Платформа для аккаунта не найдена");

            var existingNotification = await _context.BirthdayNotifications
                .FirstOrDefaultAsync(bn => bn.BirthdayId == birthdayId && bn.UserAccountId == request.UserAccountId);

            var parsedDate = DateTime.ParseExact(request.NotificationDate, "MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
            parsedDate = new DateTime(1, parsedDate.Month, parsedDate.Day, parsedDate.Hour, parsedDate.Minute, 0, DateTimeKind.Utc);

            if (existingNotification != null)
            {
                existingNotification.NotificationDate = parsedDate;
                existingNotification.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _context.Entry(existingNotification)
                .Reference(bn => bn.UserAccount)
                .LoadAsync();

                var updatedResponse = _mapper.Map<BirthdayNotificationResponse>(existingNotification);

                return ServiceResult<BirthdayNotificationResponse>.Ok(updatedResponse);
            }

            var newNotification = new BirthdayNotification()
            {
                UserAccountId = request.UserAccountId,
                NotificationDate = parsedDate,
                BirthdayId = birthdayId,
                IsSent = false,
                SentAt = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.BirthdayNotifications.Add(newNotification);
            await _context.SaveChangesAsync();

            await _context.Entry(newNotification)
                .Reference(bn => bn.UserAccount)
                .LoadAsync();

            var response = _mapper.Map<BirthdayNotificationResponse>(newNotification);
            return ServiceResult<BirthdayNotificationResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            return ServiceResult<BirthdayNotificationResponse>.Fail(500, $"Ошибка при сохранении уведомления: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteBirthdayNotificationAsync(long birthdayId, long notificationId, Guid userId)
    {
        try
        {
            var birthday = await _context.Birthdays.FirstOrDefaultAsync(b => b.Id == birthdayId && b.UserId == userId);
            if (birthday == null)
                return ServiceResult<bool>.NotFound("День рождения не найден");

            var notification = await _context.BirthdayNotifications
                .FirstOrDefaultAsync(bn => bn.Id == notificationId && bn.BirthdayId == birthdayId);

            if (notification == null)
                return ServiceResult<bool>.NotFound("Уведомление не найдено");

            _context.BirthdayNotifications.Remove(notification);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(500, $"Ошибка при удалении уведомления: {ex.Message}");
        }
    }
}