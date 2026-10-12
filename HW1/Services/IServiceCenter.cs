using HW1.Models;

namespace HW1.Services;

public interface IServiceCenter
{
    bool Inspect(IEntity entity);
}