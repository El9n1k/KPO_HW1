using HW1.Models;

namespace HW1.Services;

public class Park
{
    private readonly IRepository _repository;
    private readonly IServiceCenter _serviceCenter;

    public Park(IServiceCenter serviceCenter, IRepository repository)
    {
        _serviceCenter = serviceCenter;
        _repository = repository;
    }

    public bool Add(IEntity entity)
    {
        if (!_serviceCenter.Inspect(entity))
        {
            return false;
        }

        _repository.Add(entity);
        return true;
    }

    public bool DeleteById(int id)
    {
        return _repository.Remove(id);
    }
}