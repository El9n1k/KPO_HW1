using HW1.Models;

namespace HW1.Services.Implementations;

public class Repository : IRepository
{
    private readonly List<IEntity> _entities = [];
    private int _nextId = 1;

    public void Add(IEntity entity)
    {
        entity.Id = _nextId;
        _nextId++;
        _entities.Add(entity);
    }

    public bool Remove(int id)
    {
        int index = _entities.FindIndex(e => e.Id == id);
        if (index < 0) { return false; }

        _entities.RemoveAt(index);
        return true;
    }

    public IEntity? GetById(int id)
    {
        return _entities.FirstOrDefault(i => i.Id == id);
    }

    public IReadOnlyCollection<IEntity> GetAll()
    {
        return _entities.AsReadOnly();
    }
}