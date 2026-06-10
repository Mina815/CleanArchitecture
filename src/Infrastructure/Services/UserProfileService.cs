using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services;

public class UserProfileService : IUserProfileService
{
    private readonly ApplicationDbContext _context;

    public UserProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetFcmTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.AsNoTracking()
            .OfType<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        return user?.FcmToken;
    }

    public async Task<string?> GetOwnerIdByCenterIdAsync(int centerId, CancellationToken cancellationToken = default)
    {
        return await _context.BeautyCenters.AsNoTracking()
            .Where(c => c.Id == centerId)
            .Select(c => c.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
