namespace HW1.Models;

public interface IEnergyConsumer : IEntity
{
    double DayConsumption { get; set; }
}