using Mizuho.MyEnergySupplierPal.Domain.Entities;

namespace Mizuho.MyEnergySupplierPal.Domain.UnitTest;

public class SupplierPlanTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var energyOne = new SupplierPlan
        {
            SupplierName = "energyOne",
            PlanName = "planOne",
            Prices = new[] { new Price { Rate = 13.5f, Threshold = 100 }, new Price { Rate = 10 } }
        };
        var energyTwo = new SupplierPlan
        {
            SupplierName = "energyTwo",
            PlanName = "planTwo",
            Prices = new[] { new Price { Rate = 12.5f, Threshold = 300 }, new Price { Rate = 11 } }
        };
        var energyThree = new SupplierPlan
        {
            SupplierName = "energyThree",
            PlanName = "planThree",
            Prices = new[] { new Price { Rate = 14.5f, Threshold = 250 }, new Price { Rate = 10.1f, Threshold = 200 }, new Price { Rate = 9 } }
        };
        var energyFour = new SupplierPlan
        {
            SupplierName = "energyFour",
            PlanName = "planFour",
            Prices = new[] { new Price { Rate = 9 } },
            StandingCharge = 7
        };
        yield return new object[] { energyOne, 1000, 108.67f }; //in docs 108.68, but before rounding it is exactly: 108.674988
        yield return new object[] { energyOne, 2000, 213.68f };
        yield return new object[] { energyTwo, 1000, 120.22f };
        yield return new object[] { energyTwo, 2000, 235.72f };
        yield return new object[] { energyThree, 1000, 111.25f };
        yield return new object[] { energyThree, 2000, 205.75f };
        yield return new object[] { energyFour, 1000, 121.4f }; //in docs 121.33, but 2024 has 366 days which results in +7 pence
        yield return new object[] { energyFour, 2000, 215.9f }; //in docs 215.83, but 2024 has 366 days which results in +7 pence
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}