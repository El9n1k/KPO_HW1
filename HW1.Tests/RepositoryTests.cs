using HW1.Models.Implementations;
using HW1.Services.Implementations;

namespace HW1.Tests;

public class RepositoryTests
{
    [Fact]
    public void Add_ShouldAssignCorrectIdAndAddEntity()
    {
        // Arrange
        var repository = new Repository();
        var entity = new Bike { Name = "Test Bike", Simplicity = 5, MaxSpeed = 20 };

        // Act
        repository.Add(entity);

        // Assert
        Assert.Equal(1, entity.Id);
        Assert.Contains(entity, repository.GetAll());
    }
    
    [Fact]
    public void Add_ShouldIncrementNextIdAfterAddingEntity()
    {
        // Arrange
        var repository = new Repository();
        var entity1 = new Bike { Name = "Bike 1", Simplicity = 5, MaxSpeed = 20 };
        var entity2 = new Bike { Name = "Bike 2", Simplicity = 5, MaxSpeed = 20 };

        // Act
        repository.Add(entity1);
        repository.Add(entity2);

        // Assert
        Assert.Equal(2, entity2.Id);
    }
    
    [Fact]
    public void Remove_ShouldRemoveEntityById()
    {
        // Arrange
        var repository = new Repository();
        var entity = new Bike { Name = "Test Bike", Simplicity = 5, MaxSpeed = 20 };
        repository.Add(entity);
        var entity1 = new Bike { Name = "Test Bike 2", Simplicity = 5, MaxSpeed = 20 };
        repository.Add(entity1);
        
        // Act
        var result = repository.Remove(entity.Id);

        // Assert
        Assert.True(result);
        Assert.DoesNotContain(entity, repository.GetAll());
        Assert.Contains(entity1, repository.GetAll());
    }
    
    [Fact]
    public void Remove_ShouldReturnFalseIfEntityNotFound()
    {
        // Arrange
        var repository = new Repository();

        // Act
        var result = repository.Remove(999);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void GetById_ShouldReturnEntityIfFound()
    {
        // Arrange
        var repository = new Repository();
        var entity = new Bike { Name = "Test Bike", Simplicity = 5, MaxSpeed = 20 };
        repository.Add(entity);

        // Act
        var result = repository.GetById(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Bike", result.Name);
    }

    [Fact]
    public void GetById_ShouldReturnNullIfEntityNotFound()
    {
        // Arrange
        var repository = new Repository();

        // Act
        var result = repository.GetById(999);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GetAll_ShouldReturnAllEntities()
    {
        // Arrange
        var repository = new Repository();
        var entity1 = new Bike { Name = "Bike 1", Simplicity = 5, MaxSpeed = 20 };
        var entity2 = new Scooter { Name = "Scooter 1", Simplicity = 6, MaxSpeed = 30 };
        repository.Add(entity1);
        repository.Add(entity2);

        // Act
        var result = repository.GetAll();

        // Assert
        Assert.Contains(entity1, result);
        Assert.Contains(entity2, result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Add_AfterRemoval_ShouldNotReuseId()
    {
        var repository = new Repository();
        var first = new Bike { Name = "A" };
        repository.Add(first);
        repository.Add(new Bike { Name = "B" });
        repository.Add(new Bike { Name = "C" });

        repository.Remove(first.Id);
        var fourth = new Bike { Name = "D" };
        repository.Add(fourth);

        Assert.Equal(4, fourth.Id);
        Assert.Equal(3, repository.GetAll().Select(e => e.Id).Distinct().Count());
    }
}