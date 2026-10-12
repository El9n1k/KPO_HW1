using HW1.Models;

namespace HW1.Services;

public interface IRepository
{
    void Add(IEntity entity);
    bool Remove(int id);
    IEntity? GetById(int id);
    IReadOnlyCollection<IEntity> GetAll();
}