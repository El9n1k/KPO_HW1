using HW1.Models;
using HW1.Services;
using HW1.Models.Implementations;
using HW1.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HW1.Tests;

public class FakeServiceCenterTrue : IServiceCenter
{
    public bool Inspect(IEntity entity)
    {
        return true;
    }
}

public class FakeServiceCenterFalse : IServiceCenter
{
    public bool Inspect(IEntity entity)
    {
        return false;
    }
}

//создаю шпиона, чтобы следить о том, что park действительно вызвает ServiceCenter
public class FakeServiceCenter : IServiceCenter
{

    public bool Inspect(IEntity entity)
    {
        WasInspectCalled = true;
        return true; // Всегда возвращает true, чтобы не мешать тесту
    }
    
    public bool WasInspectCalled { get; private set; }
}

public class ParkTests
{
    [Fact]
    public void Add_ShouldReturnTrueIfServiceCenterAcceptsEntity()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter, FakeServiceCenterTrue>();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<Park>();

        var provider = services.BuildServiceProvider();
        var park = provider.GetRequiredService<Park>();

        var entity = new Bike { Name = "Test Bike", Simplicity = 11, MaxSpeed = 500 };

        // Act
        var result = park.Add(entity);

        // Assert
        Assert.True(result);
        var repository = provider.GetRequiredService<IRepository>();
        Assert.Contains(entity, repository.GetAll());
    }
    
    [Fact]
    public void Add_ShouldReturnFalseIfServiceCenterRejectsEntity()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter, FakeServiceCenterFalse>();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<Park>();

        var provider = services.BuildServiceProvider();
        var park = provider.GetRequiredService<Park>();

        var entity = new Bike { Name = "Test Bike", Simplicity = 5, MaxSpeed = 20 };

        // Act
        var result = park.Add(entity);

        // Assert
        Assert.False(result);
        var repository = provider.GetRequiredService<IRepository>();
        Assert.DoesNotContain(entity, repository.GetAll());
    }
    
    [Fact]
    public void DeleteById_ShouldReturnTrueIfEntityFound()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter, FakeServiceCenterTrue>();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<Park>();

        var provider = services.BuildServiceProvider();
        var park = provider.GetRequiredService<Park>();
        var repository = provider.GetRequiredService<IRepository>();

        var entity = new Bike { Name = "Test Bike" };
        park.Add(entity);

        // Act
        var result = park.DeleteById(entity.Id);

        // Assert
        Assert.True(result);
        Assert.DoesNotContain(entity, repository.GetAll());
    }
    
    [Fact]
    public void DeleteById_ShouldReturnFalseIfEntityNotFound()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter, FakeServiceCenterTrue>();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<Park>();

        var provider = services.BuildServiceProvider();
        var park = provider.GetRequiredService<Park>();
        var repository = provider.GetRequiredService<IRepository>();

        var entity = new Bike { Name = "Test Bike" };
        park.Add(entity);

        // Act
        var result = park.DeleteById(999);

        // Assert
        Assert.False(result);
        Assert.Contains(entity, repository.GetAll());
    }
    
    [Fact]
    public void Add_ShouldReturnFalseIfRealServiceCenterRejectsEntity()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter, ServiceCenter>(); // настоящий осмотр без подмены
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<Park>();

        var provider = services.BuildServiceProvider();
        var park = provider.GetRequiredService<Park>();

        // те же значения, что в тесте с FakeServiceCenterTrue
        var entity = new Bike { Name = "Test Bike", Simplicity = 11, MaxSpeed = 500 };

        // Act
        var result = park.Add(entity);

        // Assert
        Assert.False(result);
        var repository = provider.GetRequiredService<IRepository>();
        Assert.DoesNotContain(entity, repository.GetAll());
    }
    
    [Fact]
    public void Add_ShouldNotConsumeIdForRejectedEntity()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter, ServiceCenter>();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<Park>();

        var provider = services.BuildServiceProvider();
        var park = provider.GetRequiredService<Park>();
        var repository = provider.GetRequiredService<IRepository>();

        var invalidBike = new Bike
        {
            Name = "Invalid Bike",
            Simplicity = 5,
            MaxSpeed = 500 // MaxSpeed > 40 -> отклоняется
        };

        var validBike = new Bike
        {
            Name = "Valid Bike",
            Simplicity = 5,
            MaxSpeed = 20 // MaxSpeed ≤ 40 -> принимается
        };

        // Act: Добавляем невалидную сущность
        var result1 = park.Add(invalidBike);

        // Assert: Невалидная сущность отклонена
        Assert.False(result1);

        // Act: Добавляем валидную сущность 
        var result2 = park.Add(validBike);

        // Assert: Валидная сущность принята и получила ID = 1
        Assert.True(result2);
        Assert.Equal(1, validBike.Id);
    }
    
    [Fact]
    public void Add_ShouldCallServiceCenterInspect()
    {
        // Arrange
        var fake = new FakeServiceCenter();
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter>(fake);
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<Park>();

        var provider = services.BuildServiceProvider();
        var park = provider.GetRequiredService<Park>();

        var entity = new Bike { Name = "Test Bike", Simplicity = 5, MaxSpeed = 500 };

        // Act
        park.Add(entity);

        // Assert
        Assert.True(fake.WasInspectCalled);
    }
}