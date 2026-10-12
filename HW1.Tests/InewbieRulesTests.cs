using HW1.Models;
using HW1.Services;
using HW1.Models.Implementations;
using HW1.Services.Implementations;

namespace HW1.Tests;

public class InewbieRulesTests
{
    [Fact]
    public void IsForNewbie_ShouldReturnTrueForSimplicity7()
    {
        // Arrange
        var rules = new InewbieRules();
        var entity = new Bike { Simplicity = 7 };

        // Act
        var result = rules.IsForNewbie(entity);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void IsForNewbie_ShouldReturnTrueForSimplicity6()
    {
        // Arrange
        var rules = new InewbieRules();
        var entity = new Bike { Simplicity = 6 };

        // Act
        var result = rules.IsForNewbie(entity);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void IsForNewbie_ShouldReturnFalseForSimplicity5()
    {
        // Arrange
        var rules = new InewbieRules();
        var entity = new Bike { Simplicity = 5 };

        // Act
        var result = rules.IsForNewbie(entity);

        // Assert
        Assert.False(result);
    }
}
