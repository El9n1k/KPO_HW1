using HW1.Models;
using HW1.Models.Implementations;
using HW1.Services;
using HW1.Services.Implementations;
using HW1.UI;
using HW1.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HW1.Tests;

public class ConsoleFakeServiceCenter : IServiceCenter
{
    private readonly bool _result;

    public ConsoleFakeServiceCenter(bool result = true)
    {
        _result = result;
    }

    public List<IEntity> Inspected { get; } = new();

    public bool Inspect(IEntity entity)
    {
        Inspected.Add(entity);
        return _result;
    }
}

public class FakeEntityParser : IEntityParser
{
    private readonly IEntity _entity;

    public FakeEntityParser(string entityType, IEntity entity)
    {
        EntityType = entityType;
        _entity = entity;
    }

    public string EntityType { get; }

    public int ParseCalls { get; private set; }

    public IEntity Parse()
    {
        ParseCalls++;
        return _entity;
    }
}

public class MockReport : IReport
{
    private readonly string _text;

    public MockReport(ReportType type, string text)
    {
        Type = type;
        _text = text;
    }

    public ReportType Type { get; }

    public int CallCount { get; private set; }

    public string CreateReport()
    {
        CallCount++;
        return _text;
    }
}

[Collection("Console")]
public class ConsoleMenuTests : IDisposable
{
    private const string ValidScooter = "scooter\nXiaomi\n8\n25\n";

    private readonly TextReader _originalIn = Console.In;
    private readonly TextWriter _originalOut = Console.Out;

    public void Dispose()
    {
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);
    }

    // Настоящий набор регистраций, как в Program.cs
    private static ServiceCollection CreateRealServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<DemoRunner>();
        services.AddSingleton<IServiceCenter, ServiceCenter>();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IReport, EnergyReport>();
        services.AddSingleton<IReport, NewbieReport>();
        services.AddSingleton<IReport, EntitiesReport>();
        services.AddSingleton<IEntityParser, BikeParser>();
        services.AddSingleton<IEntityParser, EbikeParser>();
        services.AddSingleton<IEntityParser, ScooterParser>();
        services.AddSingleton<IEntityParser, HelmetParser>();
        services.AddSingleton<IEntityParser, EnergyStationParser>();
        services.AddSingleton<IInewbieRules, InewbieRules>();
        services.AddSingleton<IConsoleMenu, ConsoleMenu>();
        services.AddSingleton<Park>();
        return services;
    }

    // Подставляет ввод, гоняет меню до выхода и возвращает весь вывод
    private static string RunMenu(ServiceProvider provider, string input)
    {
        var output = new StringWriter();
        Console.SetIn(new StringReader(input));
        Console.SetOut(output);

        var menu = provider.GetRequiredService<IConsoleMenu>();
        while (menu.ProcessCommand())
        {
        }

        return output.ToString();
    }

    // add

    [Fact]
    public void Add_ValidScooter_PrintsAssignedIdAndStoresEntity()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, $"add\n{ValidScooter}quit\n");

        // Assert
        Assert.Contains("ID=1", output);
        var stored = Assert.Single(provider.GetRequiredService<IRepository>().GetAll());
        Assert.IsType<Scooter>(stored);
    }

    [Fact]
    public void Add_ScooterAboveSpeedLimit_PrintsRejectionAndStoresNothing()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "add\nscooter\nRacer\n8\n60\nquit\n");

        // Assert
        Assert.Contains("отклонили", output);
        Assert.DoesNotContain("ID=", output);
        Assert.Empty(provider.GetRequiredService<IRepository>().GetAll());
    }

    [Fact]
    public void Add_WhenInjectedInspectionRejects_PrintsRejectionEvenForValidScooter()
    {
        // Arrange
        var fake = new ConsoleFakeServiceCenter(false);
        var services = CreateRealServices();
        services.Replace(ServiceDescriptor.Singleton<IServiceCenter>(fake));
        var provider = services.BuildServiceProvider();

        // Act
        string output = RunMenu(provider, $"add\n{ValidScooter}quit\n");

        // Assert
        Assert.Contains("отклонили", output);
        Assert.Empty(provider.GetRequiredService<IRepository>().GetAll());
        Assert.IsType<Scooter>(Assert.Single(fake.Inspected));
    }

    [Fact]
    public void Add_WhenInjectedInspectionAccepts_PrintsIdEvenForValuesRealInspectionRejects()
    {
        // Arrange
        var services = CreateRealServices();
        services.Replace(ServiceDescriptor.Singleton<IServiceCenter>(new ConsoleFakeServiceCenter(true)));
        var provider = services.BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "add\nscooter\nRacer\n7\n100\nquit\n");

        // Assert
        Assert.Contains("ID=1", output);
    }

    [Fact]
    public void Add_UnknownEntityType_PrintsUnknownTypeMessage()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "add\nspaceship\nquit\n");

        // Assert
        Assert.Contains("Неизвестный тип сущности", output);
        Assert.Empty(provider.GetRequiredService<IRepository>().GetAll());
    }

    [Fact]
    public void Add_UsesParserChosenByTypeAndAddsItsEntity()
    {
        // Arrange
        var entity = new Helmet { Name = "FakeHelmet", Simplicity = 5 };
        var parser = new FakeEntityParser("Dummy", entity);
        var services = CreateRealServices();
        services.RemoveAll<IEntityParser>();
        services.AddSingleton<IEntityParser>(parser);
        var provider = services.BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "add\ndummy\nquit\n");

        // Assert
        Assert.Equal(1, parser.ParseCalls);
        Assert.Contains("ID=1", output);
        Assert.Same(entity, Assert.Single(provider.GetRequiredService<IRepository>().GetAll()));
    }

    // регистр и пробелы

    [Theory]
    [InlineData("add")]
    [InlineData("ADD")]
    [InlineData("  Add  ")]
    public void Command_IsCaseAndWhitespaceInsensitive(string command)
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, $"{command}\n{ValidScooter}quit\n");

        // Assert
        Assert.Contains("ID=1", output);
    }

    [Theory]
    [InlineData("scooter")]
    [InlineData("SCOOTER")]
    [InlineData("  Scooter  ")]
    public void EntityType_IsCaseAndWhitespaceInsensitive(string type)
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, $"add\n{type}\nXiaomi\n8\n25\nquit\n");

        // Assert
        Assert.Contains("ID=1", output);
    }

    // remove

    [Fact]
    public void Remove_ExistingId_PrintsConfirmationAndRemovesEntity()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, $"add\n{ValidScooter}remove\n1\nquit\n");

        // Assert
        Assert.Contains("Сущность удалена", output);
        Assert.Empty(provider.GetRequiredService<IRepository>().GetAll());
    }

    [Fact]
    public void Remove_UnknownId_PrintsFailureAndKeepsEntities()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, $"add\n{ValidScooter}remove\n999\nquit\n");

        // Assert
        Assert.Contains("Не получилось удалить сущность", output);
        Assert.DoesNotContain("Сущность удалена", output);
        Assert.Single(provider.GetRequiredService<IRepository>().GetAll());
    }

    // report

    [Fact]
    public void Report_PrintsRealReportWithAddedEntity()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "add\nenergystation\nDock-A\n5\n12\nreport\nenergy\nquit\n");

        // Assert
        Assert.Contains("Dock-A", output);
        Assert.Contains($"Total consumption: {12.0} kWh", output);
    }

    [Theory]
    [InlineData("energy", "Energy Report")]
    [InlineData("entity", "All entities Report")]
    [InlineData("newbie", "Entities for newbies Report")]
    [InlineData("ENERGY", "Energy Report")]
    public void Report_PrintsReportMatchingRequestedType(string type, string header)
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, $"report\n{type}\nquit\n");

        // Assert
        Assert.Contains(header, output);
    }

    [Fact]
    public void Report_CallsOnlyTheRequestedReportExactlyOnce()
    {
        // Arrange: настоящие отчёты заменены двумя mock-отчётами
        var energy = new MockReport(ReportType.Energy, "MOCK-ENERGY-TEXT");
        var entity = new MockReport(ReportType.Entity, "MOCK-ENTITY-TEXT");
        var services = CreateRealServices();
        services.RemoveAll<IReport>();
        services.AddSingleton<IReport>(energy);
        services.AddSingleton<IReport>(entity);
        var provider = services.BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "report\nenergy\nquit\n");

        // Assert
        Assert.Contains("MOCK-ENERGY-TEXT", output);
        Assert.DoesNotContain("MOCK-ENTITY-TEXT", output);
        Assert.Equal(1, energy.CallCount);
        Assert.Equal(0, entity.CallCount);
    }

    [Fact]
    public void Report_UnknownType_PrintsUnknownReportMessage()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "report\nnothing\nquit\n");

        // Assert
        Assert.Contains("Неизвестный тип отчёта.", output);
    }

    // справка, quit и конец ввода

    [Fact]
    public void UnknownCommand_PrintsHelpAndKeepsRunning()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();
        Console.SetIn(new StringReader("dance\n"));
        Console.SetOut(new StringWriter());
        var menu = provider.GetRequiredService<IConsoleMenu>();

        // Act
        bool shouldContinue = menu.ProcessCommand();

        // Assert
        Assert.True(shouldContinue);
    }

    [Fact]
    public void UnknownCommand_PrintsListOfCommands()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();

        // Act
        string output = RunMenu(provider, "dance\nquit\n");

        // Assert
        Assert.Contains("Команды: Add Quit Report Remove Demo \n", output);
    }

    [Fact]
    public void Quit_StopsTheMenu()
    {
        // Arrange
        var provider = CreateRealServices().BuildServiceProvider();
        Console.SetIn(new StringReader("quit\n"));
        Console.SetOut(new StringWriter());
        var menu = provider.GetRequiredService<IConsoleMenu>();

        // Act
        bool shouldContinue = menu.ProcessCommand();

        // Assert
        Assert.False(shouldContinue);
    }
}