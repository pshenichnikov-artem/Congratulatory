using CoreService.Core.Entities;

namespace CoreService.Infrastructure.Interfaces;

public interface IBirthdayRepository
{
    Task<IEnumerable<Birthday>> GetAllByUserIdAsync(Guid userId);
    Task<Birthday?> GetByIdAsync(int id, Guid userId);
    Task<Birthday> CreateAsync(Birthday birthday);
    Task<Birthday> UpdateAsync(Birthday birthday);
    Task<bool> DeleteAsync(int id, Guid userId);
    Task<bool> ExistsByNameAsync(string fullName, Guid userId, int? excludeId = null);
}