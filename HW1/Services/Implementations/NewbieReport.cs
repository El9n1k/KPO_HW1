using System.Globalization;
using System.Text;

using HW1.Models;

namespace HW1.Services.Implementations;

public class NewbieReport : IReport
{
    private readonly IInewbieRules _newbie;
    private readonly IRepository _repository;

    public NewbieReport(IRepository repository, IInewbieRules newbie)
    {
        _repository = repository;
        _newbie = newbie;
    }

    public ReportType Type { get; } = ReportType.Newbie;

    public string CreateReport()
    {
        List<IEntity> newbies = GetNewbies();
        return Format(newbies);
    }

    private List<IEntity> GetNewbies()
    {
        List<IEntity> newbies = [];
        foreach (IEntity entity in _repository.GetAll())
        {
            if (_newbie.IsForNewbie(entity) && entity is IMovable) { newbies.Add(entity); }
        }

        return newbies;
    }

    private static string Format(List<IEntity> newbies)
    {
        StringBuilder result = new();
        result.AppendLine(CultureInfo.InvariantCulture, $"{DateTime.Now}");
        result.AppendLine("Entities for newbies Report");
        foreach (IEntity c in newbies)
        {
            result.AppendLine(CultureInfo.InvariantCulture,
                $"Entity {c.Name} with ID={c.Id} has difficulty level {c.Simplicity}, " +
                $"which is right for newbies");
        }

        return result.ToString();
    }
}