using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public string? TitleAr { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BeautyCenter, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.TitleAr, opt => opt.MapFrom(s => s.NameAr));
            CreateMap<Branch, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.TitleAr, opt => opt.MapFrom(s => s.NameAr));
            CreateMap<Service, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.TitleAr, opt => opt.MapFrom(s => s.NameAr));
        }
    }
}
