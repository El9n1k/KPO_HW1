using System.Globalization;
using System.Text;

using HW1.Models;

namespace HW1.Services.Implementations;

public class EntitiesReport : IReport
{
    private readonly IRepository _repository;

    public EntitiesReport(IRepository repository)
    {
        _repository = repository;
    }

    public ReportType Type { get; } = ReportType.Entity;

    public string CreateReport()
    {
        List<IEntity> entities = _repository.GetAll().ToList();
        return Format(entities);
    }

    private static string Format(List<IEntity> entities)
    {
        StringBuilder result = new();
        result.AppendLine(CultureInfo.InvariantCulture, $"{DateTime.Now}");
        result.AppendLine("All entities Report");
        result.AppendLine(CultureInfo.InvariantCulture, $"Number of entities: {entities.Count}");
        foreach (IEntity c in entities)
        {
            result.AppendLine(CultureInfo.InvariantCulture,
                $"Entity {c.Name} with ID={c.Id} has difficulty level {c.Simplicity}");
        }

        return result.ToString();
    }
}