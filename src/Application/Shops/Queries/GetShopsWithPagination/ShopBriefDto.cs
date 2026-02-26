using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class ShopBriefDto
{
    public int Id { get; init; }

    public string? Name { get; init; }

    public string? Description { get; set; }    

    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public bool isOpen { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Shop, ShopBriefDto>();
        }
    }
}
