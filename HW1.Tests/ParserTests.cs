using HW1.Models.Implementations;
using HW1.UI.Implementations;

namespace HW1.Tests;

[Collection("Console")]
public class ParserTests : IDisposable
{
    private readonly TextReader _originalIn = Console.In;
    private readonly TextWriter _originalOut = Console.Out;

    public void Dispose()
    {
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);
    }

    // Подменяет ввод и возвращает writer, в который попадёт весь вывод парсера
    private static StringWriter SetConsole(string input)
    {
        Console.SetIn(new StringReader(input));
        var output = new StringWriter();
        Console.SetOut(output);
        return output;
    }

    // EntityType

    [Fact]
    public void EntityType_ShouldMatchExpectedNamesForEachParser()
    {
        Assert.Equal("Bike", new BikeParser().EntityType);
        Assert.Equal("Scooter", new ScooterParser().EntityType);
        Assert.Equal("Ebike", new EbikeParser().EntityType);
        Assert.Equal("Helmet", new HelmetParser().EntityType);
        Assert.Equal("EnergyStation", new EnergyStationParser().EntityType);
    }

    // Тип результата и поля

    [Fact]
    public void BikeParser_ShouldReturnBikeWithEnteredFields()
    {
        // Arrange
        SetConsole("CityBike\n9\n25\n");

        // Act
        var result = new BikeParser().Parse();

        // Assert
        var bike = Assert.IsType<Bike>(result);
        Assert.Equal("CityBike", bike.Name);
        Assert.Equal(9, bike.Simplicity);
        Assert.Equal(25, bike.MaxSpeed);
    }

    [Fact]
    public void ScooterParser_ShouldReturnScooterWithEnteredFields()
    {
        // Arrange
        SetConsole("Xiaomi\n8\n30\n");

        // Act
        var result = new ScooterParser().Parse();

        // Assert
        var scooter = Assert.IsType<Scooter>(result); // IsType ловит случай «ScooterParser создаёт Bike»
        Assert.Equal("Xiaomi", scooter.Name);
        Assert.Equal(8, scooter.Simplicity);
        Assert.Equal(30, scooter.MaxSpeed);
    }

    [Fact]
    public void EbikeParser_ShouldReturnEBikeWithEnteredFields()
    {
        // Arrange
        SetConsole("E-Cargo\n4\n35\n1.5\n");

        // Act
        var result = new EbikeParser().Parse();

        // Assert
        var ebike = Assert.IsType<EBike>(result);
        Assert.Equal("E-Cargo", ebike.Name);
        Assert.Equal(4, ebike.Simplicity);
        Assert.Equal(35, ebike.MaxSpeed);
        Assert.Equal(1.5, ebike.DayConsumption);
    }

    [Fact]
    public void HelmetParser_ShouldReturnHelmetWithEnteredFields()
    {
        // Arrange
        SetConsole("Safety\n7\n");

        // Act
        var result = new HelmetParser().Parse();

        // Assert
        var helmet = Assert.IsType<Helmet>(result);
        Assert.Equal("Safety", helmet.Name);
        Assert.Equal(7, helmet.Simplicity);
    }

    [Fact]
    public void EnergyStationParser_ShouldReturnEnergyStationWithEnteredFields()
    {
        // Arrange
        SetConsole("Dock-A\n5\n12\n");

        // Act
        var result = new EnergyStationParser().Parse();

        // Assert
        var station = Assert.IsType<EnergyStation>(result);
        Assert.Equal("Dock-A", station.Name);
        Assert.Equal(5, station.Simplicity);
        Assert.Equal(12.0, station.DayConsumption);
    }

    // Неверный ввод повторяет запрос

    [Fact]
    public void ScooterParser_ShouldAskAgainWhenSimplicityIsNotANumber()
    {
        // Arrange: «abc» отвергается, берётся следующее значение
        var output = SetConsole("Xiaomi\nabc\n8\n25\n");

        // Act
        var scooter = Assert.IsType<Scooter>(new ScooterParser().Parse());

        // Assert
        Assert.Equal(8, scooter.Simplicity);
        Assert.Contains("Введите целое число от 1 до 10.", output.ToString());
    }

    [Fact]
    public void BikeParser_ShouldAskAgainWhenValuesAreOutOfRange()
    {
        // Arrange: простота 0 и 11 вне диапазона 1-10, скорость 0 и 101 вне диапазона 1-100
        SetConsole("CityBike\n0\n11\n5\n0\n101\n20\n");

        // Act
        var bike = Assert.IsType<Bike>(new BikeParser().Parse());

        // Assert
        Assert.Equal(5, bike.Simplicity);
        Assert.Equal(20, bike.MaxSpeed);
    }

    [Fact]
    public void BikeParser_ShouldAskAgainWhenNameIsEmptyOrWhitespace()
    {
        // Arrange: пустая строка и строка из пробелов отвергаются
        var output = SetConsole("\n   \nCityBike\n9\n20\n");

        // Act
        var bike = Assert.IsType<Bike>(new BikeParser().Parse());

        // Assert
        Assert.Equal("CityBike", bike.Name);
        Assert.Contains("Значение не может быть пустым.", output.ToString());
    }

    [Fact]
    public void HelmetParser_ShouldTrimSpacesAroundName()
    {
        // Arrange
        SetConsole("  Safety  \n7\n");

        // Act
        var helmet = Assert.IsType<Helmet>(new HelmetParser().Parse());

        // Assert
        Assert.Equal("Safety", helmet.Name);
    }

    [Fact]
    public void EbikeParser_ShouldAskAgainWhenConsumptionIsInvalidOrNegative()
    {
        // Arrange: текст и отрицательное число отвергаются
        var output = SetConsole("E-Cargo\n4\n35\nabc\n-1\n2\n");

        // Act
        var ebike = Assert.IsType<EBike>(new EbikeParser().Parse());

        // Assert
        Assert.Equal(2.0, ebike.DayConsumption);
        Assert.Contains("Введите неотрицательное число", output.ToString());
    }

    [Fact]
    public void EnergyStationParser_ShouldAskAgainWhenConsumptionIsNegative()
    {
        // Arrange
        SetConsole("Dock-A\n5\n-3\n12\n");

        // Act
        var station = Assert.IsType<EnergyStation>(new EnergyStationParser().Parse());

        // Assert
        Assert.Equal(12.0, station.DayConsumption);
    }

    // Дробный расход: точка и запятая

    [Theory]
    [InlineData("1.5")]
    [InlineData("1,5")]
    public void EbikeParser_ShouldAcceptFractionalConsumptionWithDotOrComma(string input)
    {
        // Arrange
        SetConsole($"E-Cargo\n4\n35\n{input}\n");

        // Act
        var ebike = Assert.IsType<EBike>(new EbikeParser().Parse());

        // Assert
        Assert.Equal(1.5, ebike.DayConsumption);
    }

    [Theory]
    [InlineData("12.75")]
    [InlineData("12,75")]
    public void EnergyStationParser_ShouldAcceptFractionalConsumptionWithDotOrComma(string input)
    {
        // Arrange
        SetConsole($"Dock-A\n5\n{input}\n");

        // Act
        var station = Assert.IsType<EnergyStation>(new EnergyStationParser().Parse());

        // Assert
        Assert.Equal(12.75, station.DayConsumption);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("0.0")]
    public void EnergyStationParser_ShouldAcceptZeroConsumption(string input)
    {
        // Arrange: граница: расход 0 допустим
        SetConsole($"Dock-A\n5\n{input}\n");

        // Act
        var station = Assert.IsType<EnergyStation>(new EnergyStationParser().Parse());

        // Assert
        Assert.Equal(0.0, station.DayConsumption);
    }
}