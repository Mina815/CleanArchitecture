namespace CleanArchitecture.Application.Notifications.Queries;

public class NotificationDto
{
    public int Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? ActionUrl { get; init; }
    public bool IsRead { get; init; }
    public DateTimeOffset? ReadAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
        }
    }
}
