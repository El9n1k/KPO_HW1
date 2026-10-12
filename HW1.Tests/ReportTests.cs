using HW1.Models;
using HW1.Services;
using HW1.Models.Implementations;
using HW1.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HW1.Tests;

public class EnergyReportTests
{
    [Fact]
    public void Type_ShouldBeEnergy()
    {
        var report = new EnergyReport(new Repository());

        Assert.Equal(ReportType.Energy, report.Type);
    }

    [Fact]
    public void CreateReport_ShouldSumConsumptionOfAllEnergyConsumers()
    {
        // Arrange
        var repository = new Repository();
        repository.Add(new EBike { Name = "E-Cargo", Simplicity = 4, MaxSpeed = 30, DayConsumption = 1.5 });
        repository.Add(new EnergyStation { Name = "Dock-A", Simplicity = 5, DayConsumption = 12 });
        var report = new EnergyReport(repository);

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.Contains($"Total consumption: 13.5 kWh", result);
    }

    [Fact]
    public void CreateReport_ShouldListEachConsumerWithIdAndConsumption()
    {
        // Arrange
        var repository = new Repository();
        var ebike = new EBike { Name = "E-Cargo", Simplicity = 4, MaxSpeed = 30, DayConsumption = 1.5 };
        var station = new EnergyStation { Name = "Dock-A", Simplicity = 5, DayConsumption = 12 };
        repository.Add(ebike);
        repository.Add(station);
        var report = new EnergyReport(repository);

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.Contains($"Entity E-Cargo with ID={ebike.Id} consumes 1.5 kWh per day", result);
        Assert.Contains($"Entity Dock-A with ID={station.Id} consumes {12.0} kWh per day", result);
    }

    [Fact]
    public void CreateReport_ShouldNotIncludeEntitiesThatDoNotConsumeEnergy()
    {
        // Arrange
        var repository = new Repository();
        repository.Add(new EnergyStation { Name = "Dock-A", Simplicity = 5, DayConsumption = 12 });
        repository.Add(new Helmet { Name = "HelmetOne", Simplicity = 9 });
        repository.Add(new Bike { Name = "PlainBike", Simplicity = 9, MaxSpeed = 20 });
        var report = new EnergyReport(repository);

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.DoesNotContain("HelmetOne", result);
        Assert.DoesNotContain("PlainBike", result);
        Assert.Contains($"Total consumption: {12.0} kWh", result);
    }

    [Fact]
    public void CreateReport_ShouldShowZeroTotalForEmptyPark()
    {
        // Arrange
        var report = new EnergyReport(new Repository());

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.Contains($"Total consumption: {0} kWh", result);
        Assert.DoesNotContain("with ID=", result);
    }

    [Fact]
    public void CreateReport_ShouldReflectChangesMadeAfterReportWasCreated()
    {
        // Arrange
        var repository = new Repository();
        var report = new EnergyReport(repository);
        repository.Add(new EnergyStation { Name = "Dock-A", Simplicity = 5, DayConsumption = 10 });
        string first = report.CreateReport();

        repository.Add(new EnergyStation { Name = "Dock-B", Simplicity = 5, DayConsumption = 5 });

        // Act
        string second = report.CreateReport();

        // Assert
        Assert.Contains($"Total consumption: {10.0} kWh", first);
        Assert.Contains($"Total consumption: {15.0} kWh", second);
    }
}


public class EntitiesReportTests
{
    [Fact]
    public void Type_ShouldBeEntity()
    {
        var report = new EntitiesReport(new Repository());

        Assert.Equal(ReportType.Entity, report.Type);
    }

    [Fact]
    public void CreateReport_ShouldListAllKindsOfEntitiesWithIds()
    {
        // Arrange
        var repository = new Repository();
        var bike = new Bike { Name = "CityBike", Simplicity = 9, MaxSpeed = 20 };
        var scooter = new Scooter { Name = "Xiaomi", Simplicity = 8, MaxSpeed = 25 };
        var ebike = new EBike { Name = "E-Cargo", Simplicity = 4, MaxSpeed = 30, DayConsumption = 1.5 };
        var helmet = new Helmet { Name = "HelmetOne", Simplicity = 9 };
        var station = new EnergyStation { Name = "Dock-A", Simplicity = 5, DayConsumption = 12 };
        repository.Add(bike);
        repository.Add(scooter);
        repository.Add(ebike);
        repository.Add(helmet);
        repository.Add(station);
        var report = new EntitiesReport(repository);

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.Contains($"Entity CityBike with ID={bike.Id}", result);
        Assert.Contains($"Entity Xiaomi with ID={scooter.Id}", result);
        Assert.Contains($"Entity E-Cargo with ID={ebike.Id}", result);
        Assert.Contains($"Entity HelmetOne with ID={helmet.Id}", result);
        Assert.Contains($"Entity Dock-A with ID={station.Id}", result);
    }

    [Fact]
    public void CreateReport_ShouldNotListRemovedEntity()
    {
        // Arrange
        var repository = new Repository();
        var removed = new Bike { Name = "RemovedBike", Simplicity = 5, MaxSpeed = 20 };
        repository.Add(removed);
        repository.Add(new Bike { Name = "KeptBike", Simplicity = 5, MaxSpeed = 20 });
        repository.Remove(removed.Id);
        var report = new EntitiesReport(repository);

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.DoesNotContain("RemovedBike", result);
        Assert.Contains("KeptBike", result);
    }

    [Fact]
    public void CreateReport_ShouldNotFailForEmptyPark()
    {
        // Arrange
        var report = new EntitiesReport(new Repository());

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.DoesNotContain("with ID=", result);
    }
}

public class StubNewbieRules : IInewbieRules
{
    private readonly bool _result;

    public StubNewbieRules(bool result)
    {
        _result = result;
    }

    public bool IsForNewbie(IEntity entity)
    {
        return _result;
    }
}

public class NewbieReportTests
{
    [Fact]
    public void Type_ShouldBeNewbie()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules, InewbieRules>();
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var report = provider.GetRequiredService<NewbieReport>();

        // Assert
        Assert.Equal(ReportType.Newbie, report.Type);
    }

    [Fact]
    public void CreateReport_ShouldIncludeMovableWithSimplicitySixAndExcludeFive()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules, InewbieRules>();
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IRepository>();
        var report = provider.GetRequiredService<NewbieReport>();

        var six = new Scooter { Name = "ScooterSix", Simplicity = 6, MaxSpeed = 25 };
        repository.Add(six);
        repository.Add(new Scooter { Name = "ScooterFive", Simplicity = 5, MaxSpeed = 25 });

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.Contains($"Entity ScooterSix with ID={six.Id}", result);
        Assert.DoesNotContain("ScooterFive", result);
    }

    [Fact]
    public void CreateReport_ShouldExcludeHelmetEvenWithHighSimplicity()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules, InewbieRules>();
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IRepository>();
        var report = provider.GetRequiredService<NewbieReport>();

        repository.Add(new Helmet { Name = "HelmetOne", Simplicity = 9 });
        repository.Add(new Bike { Name = "CityBike", Simplicity = 9, MaxSpeed = 20 });

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.DoesNotContain("HelmetOne", result);
        Assert.Contains("CityBike", result);
    }

    [Fact]
    public void CreateReport_ShouldExcludeEnergyStationEvenWithHighSimplicity()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules, InewbieRules>();
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IRepository>();
        var report = provider.GetRequiredService<NewbieReport>();

        repository.Add(new EnergyStation { Name = "Dock-A", Simplicity = 10, DayConsumption = 5 });

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.DoesNotContain("Dock-A", result);
    }

    [Fact]
    public void CreateReport_ShouldIncludeEBike()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules, InewbieRules>();
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IRepository>();
        var report = provider.GetRequiredService<NewbieReport>();

        repository.Add(new EBike { Name = "E-Easy", Simplicity = 8, MaxSpeed = 25, DayConsumption = 1 });

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.Contains("E-Easy", result);
    }

    [Fact]
    public void CreateReport_WhenRulesAcceptEverything_StillExcludesNonMovable()
    {
        // Arrange: вместо настоящего правила в контейнер подставлена заглушка
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules>(new StubNewbieRules(true));
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IRepository>();
        var report = provider.GetRequiredService<NewbieReport>();

        repository.Add(new Helmet { Name = "HelmetOne", Simplicity = 1 });
        repository.Add(new Scooter { Name = "HardScooter", Simplicity = 1, MaxSpeed = 25 });

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.Contains("HardScooter", result); // простота 1 не мешает, решает заглушка
        Assert.DoesNotContain("HelmetOne", result); // шлем отсекается типом
    }

    [Fact]
    public void CreateReport_WhenRulesRejectEverything_ReturnsNoEntities()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules>(new StubNewbieRules(false));
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IRepository>();
        var report = provider.GetRequiredService<NewbieReport>();

        repository.Add(new Scooter { Name = "EasyScooter", Simplicity = 10, MaxSpeed = 25 });

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.DoesNotContain("EasyScooter", result);
        Assert.DoesNotContain("with ID=", result);
    }

    [Fact]
    public void CreateReport_ShouldNotFailForEmptyPark()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IInewbieRules, InewbieRules>();
        services.AddTransient<NewbieReport>();
        var provider = services.BuildServiceProvider();
        var report = provider.GetRequiredService<NewbieReport>();

        // Act
        string result = report.CreateReport();

        // Assert
        Assert.DoesNotContain("with ID=", result);
    }
}