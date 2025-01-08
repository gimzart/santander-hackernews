using FluentAssertions;
using Mizuho.MyEnergySupplierPal.AppConsole.Services;
using Mizuho.MyEnergySupplierPal.Application.Commands.CalculateAnnualCost;
using Mizuho.MyEnergySupplierPal.Application.Commands.LoadPlans;
using Mizuho.MyEnergySupplierPal.Application.Exceptions;

namespace Mizuho.MyEnergySupplierPal.AppConsole.UnitTest.Services;
public class UserInteractionServiceTests
{
    private readonly IUserInteractionService _userInteractionService;

    public UserInteractionServiceTests()
    {
        _userInteractionService = new UserInteractionService();
    }

    [Fact]
    public void When_ProcessUserInputWithValidInputCommand_Should_CreateLoadPlansCommand()
    {
        var result = _userInteractionService.ProcessUserInput("input TestInput/plans.json");
        result.Should().NotBeNull();
        result.Should().BeOfType<LoadPlansCommand>();
        result.As<LoadPlansCommand>().FilePath.Should().Be("TestInput/plans.json");
    }

    [Fact]
    public void When_ProcessUserInputWithValidAnnualCostCommand_Should_CreateCalculateAnnualCostCommand()
    {
        var result = _userInteractionService.ProcessUserInput("annual_cost 1000");
        result.Should().NotBeNull();
        result.Should().BeOfType<CalculateAnnualCostCommand>();
        result.As<CalculateAnnualCostCommand>().AnnualConsumption.Should().Be(1000);
    }

    [Theory]
    [InlineData(" ", "Blank lines cannot be processed")]
    [InlineData("input", "Wrong number of arguments, valid example: input C:\\Users\\Public\\TestFolder\\plans.json")]
    [InlineData("input a b", "Wrong number of arguments, valid example: input C:\\Users\\Public\\TestFolder\\plans.json")]
    [InlineData("input somefile.json", "File does not exist")]
    [InlineData("annual_cost", "Wrong number of arguments, valid example: annual_cost 5000")]
    [InlineData("annual_cost a b", "Wrong number of arguments, valid example: annual_cost 5000")]
    [InlineData("annual_cost a", "Argument must be an integer value")]
    [InlineData("some_command", "User input not recognized")]

    public void When_ProcessUserInputWithInvalidCommand_Should_ThrowValidationException(string userInput, string validationMessage)
    {
        var act = () => _userInteractionService.ProcessUserInput(userInput);
        act.Should().Throw<ValidationException>().WithMessage(validationMessage);
    }
}
