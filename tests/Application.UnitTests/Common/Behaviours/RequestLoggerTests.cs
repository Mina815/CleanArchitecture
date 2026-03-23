using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.ServiceCategories.Queries.GetServiceCategories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<GetServiceCategoriesQuery>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<GetServiceCategoriesQuery>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceWhenAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<GetServiceCategoriesQuery>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new GetServiceCategoriesQuery(), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceWhenUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<GetServiceCategoriesQuery>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new GetServiceCategoriesQuery(), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
