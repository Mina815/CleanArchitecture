using System.Runtime.CompilerServices;
using AutoMapper;
using CleanArchitecture.Application.Bookings.Queries.GetMyBookings;
using CleanArchitecture.Application.Branches.Queries.GetCenterBranches;
using CleanArchitecture.Application.Centers.Queries.GetCenters;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Services.Queries.GetCenterServices;
using CleanArchitecture.Domain.Entities;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private ILoggerFactory? _loggerFactory;
    private MapperConfiguration? _configuration;
    private IMapper? _mapper;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _loggerFactory = LoggerFactory.Create(b => b.AddDebug().SetMinimumLevel(LogLevel.Debug));

        _configuration = new MapperConfiguration(cfg =>
            cfg.AddMaps(typeof(IApplicationDbContext).Assembly),
            loggerFactory: _loggerFactory);

        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration!.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(BeautyCenter), typeof(CenterDto))]
    [TestCase(typeof(Branch), typeof(BranchDto))]
    [TestCase(typeof(Domain.Entities.Service), typeof(ServiceDto))]
    [TestCase(typeof(Booking), typeof(BookingDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);
        _mapper!.Map(instance, source, destination);
    }

    private static object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        return RuntimeHelpers.GetUninitializedObject(type);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _loggerFactory?.Dispose();
    }
}
