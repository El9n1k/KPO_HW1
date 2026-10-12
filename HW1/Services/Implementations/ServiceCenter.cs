using HW1.Models;

namespace HW1.Services.Implementations;

public class ServiceCenter : IServiceCenter
{
    public bool Inspect(IEntity entity)
    {
        if (entity.Simplicity is > 10 or < 1) { return false; }

        if (entity is IEnergyConsumer energyConsumer)
        {
            if (energyConsumer.DayConsumption is > 1000000 or < 0) { return false; }
        }

        if (entity is IMovable movable)
        {
            if (movable.MaxSpeed is > 40 or < 0) { return false; }
        }

        //можно вписать другие правила
        return true;
    }
}