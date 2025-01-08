using FluentAssertions;
using Mizuho.MyEnergySupplierPal.Application.Repositories;
using Mizuho.MyEnergySupplierPal.Domain.Entities;
using Mizuho.MyEnergySupplierPal.Infrastructure.Repositories;
using Mizuho.MyEnergySupplierPal.Infrastructure.Storage;
using NSubstitute;

namespace Mizuho.MyEnergySupplierPal.Infrastructure.UnitTest.Repositories;
public class SupplierPlanRepositoryTests
{
    private readonly IStorageProvider _storageProvider = Substitute.For<IStorageProvider>();
    private readonly ISupplierPlanRepository _supplierPlanRepository;

    public SupplierPlanRepositoryTests()
    {
        _supplierPlanRepository = new SupplierPlanRepository(_storageProvider);
    }

    [Fact]
    public void When_InitializeSupplierPlansCalled_Should_ClearStorageAndSetNewItems()
    {
        var newSupplierPlans = new List<SupplierPlan> { new SupplierPlan { PlanName = "" } };
        var supplierPlans = new List<SupplierPlan>();
        _storageProvider.SupplierPlans.Returns(supplierPlans);

        _supplierPlanRepository.InitializeSupplierPlans(newSupplierPlans);
        _storageProvider.SupplierPlans.Should().NotBeNullOrEmpty();
        _storageProvider.SupplierPlans.Count().Should().Be(newSupplierPlans.Count());
    }
}
