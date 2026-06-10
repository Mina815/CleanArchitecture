namespace CleanArchitecture.Application.Common.Interfaces;

public interface IUserProfileService
{
    Task<string?> GetFcmTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<string?> GetOwnerIdByCenterIdAsync(int centerId, CancellationToken cancellationToken = default);
}
