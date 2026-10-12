namespace HW1.Models;

public interface IMovable : IEntity
{
    int MaxSpeed { get; set; }
}