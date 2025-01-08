using FluentAssertions;
using Mizuho.MyEnergySupplierPal.Domain.Entities;

namespace Mizuho.MyEnergySupplierPal.Domain.UnitTest.Entities;
public class SupplierPlanTests
{
    [Theory]
    [ClassData(typeof(SupplierPlanTestData))]
    public void When_CalculateAnnualCostCalled_Should_ReturnProperValue(SupplierPlan supplierPlan, int annualConsumption, float annualCost)
    {
        var calculatedAnnualCost1000 = supplierPlan.CalculateAnnualCost(annualConsumption);
        calculatedAnnualCost1000.Should().Be(annualCost);
    }
}
