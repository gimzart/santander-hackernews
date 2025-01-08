using System.Globalization;
using FluentAssertions;
using Mizuho.MyEnergySupplierPal.Application.Commands.CalculateAnnualCost;
using Mizuho.MyEnergySupplierPal.Application.Repositories;
using Mizuho.MyEnergySupplierPal.Domain.Entities;
using NSubstitute;

namespace Mizuho.MyEnergySupplierPal.Application.UnitTest.Commands.CalculateAnnualCost;
public class CalculateAnnualCostCommandHandlerTests
{
    private readonly ISupplierPlanRepository _supplierPlanRepository = Substitute.For<ISupplierPlanRepository>();
    private readonly CalculateAnnualCostCommandHandler calculateAnnualCostCommandHandler;

    public CalculateAnnualCostCommandHandlerTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        calculateAnnualCostCommandHandler = new CalculateAnnualCostCommandHandler(_supplierPlanRepository);
    }

    [Fact]
    public async Task When_CalculateAnnualCostCommandHandlerCalled_Should_LoadSupplierPlansCalculateAnnualCostAndReturnSortedData()
    {
        var energyOne = new SupplierPlan
        {
            SupplierName = "energyOne",
            PlanName = "planOne",
            Prices = [new Price { Rate = 13.5f, Threshold = 100 }, new Price { Rate = 10 }]
        };
        var command = new CalculateAnnualCostCommand(1000);
        _supplierPlanRepository.GetAllSupplierPlans().Returns([energyOne]);

        var result = await calculateAnnualCostCommandHandler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().Be("energyOne,planOne,108.67");
        _supplierPlanRepository.Received().GetAllSupplierPlans();
    }
}
