using FluentAssertions;
using Mizuho.MyEnergySupplierPal.Application.Commands.LoadPlans;
using Mizuho.MyEnergySupplierPal.Application.Exceptions;
using Mizuho.MyEnergySupplierPal.Application.ExternalServices;
using Mizuho.MyEnergySupplierPal.Application.Repositories;
using Mizuho.MyEnergySupplierPal.Domain.Entities;
using NSubstitute;

namespace Mizuho.MyEnergySupplierPal.Application.UnitTest.Commands.LoadPlans;
public class LoadPlansCommandHandlerTests
{
    private readonly IFileInputProvider _fileInputProvider = Substitute.For<IFileInputProvider>();
    private readonly ISupplierPlanRepository _supplierPlanRepository = Substitute.For<ISupplierPlanRepository>();
    private readonly LoadPlansCommandHandler _loadPlansCommandHandler;
    private readonly string _plansFilePath = "TestInput/plans.json";
    private readonly string _schemaFilePath = "Schemas/plan_schema.json";
    private readonly string _plansJson = "[\r\n    {\r\n        \"supplier_name\": \"energyOne\",\r\n        \"plan_name\": \"planOne\",\r\n        \"prices\": [\r\n            {\r\n                \"rate\": 13.5,\r\n                \"threshold\": 100\r\n            },\r\n            { \"rate\": 10 }\r\n        ]\r\n    },\r\n    {\r\n        \"supplier_name\": \"energyTwo\",\r\n        \"plan_name\": \"planTwo\",\r\n        \"prices\": [\r\n            {\r\n                \"rate\": 12.5,\r\n                \"threshold\": 300\r\n            },\r\n            { \"rate\": 11 }\r\n        ]\r\n    },\r\n    {\r\n        \"supplier_name\": \"energyThree\",\r\n        \"plan_name\": \"planThree\",\r\n        \"prices\": [\r\n            {\r\n                \"rate\": 14.5,\r\n                \"threshold\": 250\r\n            },\r\n            {\r\n                \"rate\": 10.1,\r\n                \"threshold\": 200\r\n            },\r\n            { \"rate\": 9 }\r\n        ]\r\n    },\r\n    {\r\n        \"supplier_name\": \"energyFour\",\r\n        \"plan_name\": \"planFour\",\r\n        \"prices\": [ { \"rate\": 9 } ],\r\n        \"standing_charge\": 7\r\n    }\r\n]";
    private readonly string _plansJsonInvalid = "[\r\n    {\r\n        \"supplier_name\": \"energyOne\",\r\n        \"plan_name\": \"planOne\",\r\n        \"prices\": [\r\n            {\r\n                \"rate\": \"not a value\",\r\n                \"threshold\": 100\r\n            },\r\n            { \"rate\": 10 }\r\n        ]\r\n    },\r\n    {\r\n        \"supplier_name\": \"energyTwo\",\r\n        \"plan_name\": \"planTwo\",\r\n        \"prices\": [\r\n            {\r\n                \"rate\": 12.5,\r\n                \"threshold\": 300\r\n            },\r\n            { \"rate\": 11 }\r\n        ]\r\n    },\r\n    {\r\n        \"supplier_name\": \"energyThree\",\r\n        \"plan_name\": \"planThree\",\r\n        \"prices\": [\r\n            {\r\n                \"rate\": 14.5,\r\n                \"threshold\": 250\r\n            },\r\n            {\r\n                \"rate\": 10.1,\r\n                \"threshold\": 200\r\n            },\r\n            { \"rate\": 9 }\r\n        ]\r\n    },\r\n    {\r\n        \"supplier_name\": \"energyFour\",\r\n        \"plan_name\": \"planFour\",\r\n        \"prices\": [ { \"rate\": 9 } ],\r\n        \"standing_charge\": 7\r\n    }\r\n]";
    private readonly string _planSchemaJson = "{\r\n    \"$schema\": \"http://json-schema.org/myenergypal/schema#\",\r\n    \"description\": \"Schema for validating a Plan instance.\",\r\n    \"id\": \"https://www.mizuho-emea.com/myenergypalschema.json\",\r\n    \"properties\": {\r\n        \"plan_name\": {\r\n            \"description\": \"Unique Plan Name\",\r\n            \"id\": \"/properties/plan_name\",\r\n            \"title\": \"Plan Name\",\r\n            \"type\": \"string\"\r\n        },\r\n        \"prices\": {\r\n            \"additionalItems\": false,\r\n            \"id\": \"/properties/prices\",\r\n            \"items\": {\r\n                \"id\": \"/properties/prices/items\",\r\n                \"properties\": {\r\n                    \"rate\": {\r\n                        \"description\": \"Rate expressed in pence; exclusive of VAT\",\r\n                        \"id\": \"/properties/prices/items/properties/rate\",\r\n                        \"title\": \"Rate\",\r\n                        \"type\": \"number\"\r\n                    },\r\n                    \"threshold\": {\r\n                        \"description\": \"Energy to be consumed at this rate annually\",\r\n                        \"id\": \"/properties/prices/items/properties/threshold\",\r\n                        \"title\": \"Threshold\",\r\n                        \"type\": \"integer\"\r\n                    }\r\n                },\r\n                \"type\": \"object\"\r\n            },\r\n            \"type\": \"array\"\r\n        },\r\n        \"standing_charge\": {\r\n            \"description\": \"Daily charge expressed in pence; exclusive of VAT\",\r\n            \"id\": \"/properties/standing_charge\",\r\n            \"title\": \"Standing Charge\",\r\n            \"type\": \"integer\"\r\n        },\r\n        \"supplier_name\": {\r\n            \"description\": \"Supplier Name\",\r\n            \"id\": \"/properties/supplier_name\",\r\n            \"title\": \"Supplier Name\",\r\n            \"type\": \"string\"\r\n        }\r\n    },\r\n    \"title\": \"Root JSON Schema.\",\r\n    \"type\": \"array\"\r\n}";

    public LoadPlansCommandHandlerTests()
    {
        _loadPlansCommandHandler = new LoadPlansCommandHandler(_fileInputProvider, _supplierPlanRepository);
    }

    [Fact]
    public async Task When_LoadPlansCommandHandlerCalled_Should_ValidateDeserializeAndStoreSupplierPlans()
    {
        var command = new LoadPlansCommand(_plansFilePath);
        _fileInputProvider.GetFileInput(_plansFilePath).Returns(_plansJson);
        _fileInputProvider.GetFileInput(_schemaFilePath).Returns(_planSchemaJson);

        var result = await _loadPlansCommandHandler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().Be("Loaded 4 plans");
        await _fileInputProvider.Received().GetFileInput(_plansFilePath);
        await _fileInputProvider.Received().GetFileInput(_schemaFilePath);
        _supplierPlanRepository.Received().InitializeSupplierPlans(Arg.Is<IEnumerable<SupplierPlan>>(x => x.Count() == 4));
    }

    [Fact]
    public async Task When_LoadPlansCommandHandlerCalledWithInvalidPlan_Should_ThrowValidationException()
    {
        var command = new LoadPlansCommand(_plansFilePath);
        _fileInputProvider.GetFileInput(_plansFilePath).Returns(_plansJsonInvalid);
        _fileInputProvider.GetFileInput(_schemaFilePath).Returns(_planSchemaJson);

        var act = async () => await _loadPlansCommandHandler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();
        _supplierPlanRepository.DidNotReceive().InitializeSupplierPlans(Arg.Is<IEnumerable<SupplierPlan>>(x => x.Count() == 4));
    }
}
