using CleanArchitecture.Application.ServiceCategories.Queries.GetServiceCategories;
using CleanArchitecture.Domain.Entities;
using Shouldly;

namespace CleanArchitecture.Application.FunctionalTests.ServiceCategories.Queries;

public class GetServiceCategoriesTests : TestBase
{
    [Test]
    public async Task ShouldReturnActiveCategories()
    {
        await TestApp.AddAsync(new ServiceCategory
        {
            Name = "Nails",
            NameAr = "أظافر",
            DisplayOrder = 2,
            IsActive = true
        });

        var list = await TestApp.SendAsync(new GetServiceCategoriesQuery());

        list.ShouldNotBeEmpty();
        list.ShouldContain(c => c.Name == "Nails");
    }
}
