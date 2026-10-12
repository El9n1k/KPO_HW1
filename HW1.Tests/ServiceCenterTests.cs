using HW1.Models;
using HW1.Services;
using HW1.Models.Implementations;
using HW1.Services.Implementations;

namespace HW1.Tests;

public class ServiceCenterTests
{
    [Fact]
    public void Inspect_ShouldReturnTrueForValidEntity()
    {
        // Arrange
        var serviceCenter = new ServiceCenter();
        var bike = new Bike
        {
            Simplicity = 5,
            MaxSpeed = 20
        };

        // Act
        var result = serviceCenter.Inspect(bike);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Inspect_ShouldReturnFalseIfSimplicityIsInvalid()
    {
        // Arrange
        var serviceCenter = new ServiceCenter();
        var bike = new Bike
        {
            Simplicity = 11,
            MaxSpeed = 20
        };

        // Act
        var result = serviceCenter.Inspect(bike);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Inspect_ShouldReturnFalseIfMaxSpeedIsInvalid()
    {
        // Arrange
        var serviceCenter = new ServiceCenter();
        var bike = new Bike
        {
            Simplicity = 5,
            MaxSpeed = 50
        };

        // Act
        var result = serviceCenter.Inspect(bike);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Inspect_ShouldReturnFalseIfDayConsumptionIsInvalid()
    {
        // Arrange
        var serviceCenter = new ServiceCenter();
        var energyStation = new EnergyStation
        {
            Simplicity = 5,
            DayConsumption = -1
        };

        // Act
        var result = serviceCenter.Inspect(energyStation);

        // Assert
        Assert.False(result);
    }
}