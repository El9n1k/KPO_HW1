using System.Globalization;
using System.Text;

using HW1.Models;

namespace HW1.Services.Implementations;

public class EnergyReport : IReport
{
    private readonly IRepository _repository;

    public EnergyReport(IRepository repository)
    {
        _repository = repository;
    }

    public ReportType Type { get; } = ReportType.Energy;

    public string CreateReport()
    {
        List<IEnergyConsumer> consumers = GetConsumers();
        double total = consumers.Sum(c => c.DayConsumption);
        return Format(consumers, total);
    }

    private List<IEnergyConsumer> GetConsumers()
    {
        return _repository.GetAll().OfType<IEnergyConsumer>().ToList();
    }

    private static string Format(List<IEnergyConsumer> consumers, double total)
    {
        StringBuilder result = new();
        result.AppendLine(CultureInfo.InvariantCulture, $"{DateTime.Now}");
        result.AppendLine("Energy Report");
        result.AppendLine(CultureInfo.InvariantCulture, $"Total consumption: {total} kWh");
        foreach (IEnergyConsumer c in consumers)
        {
            result.AppendLine(CultureInfo.InvariantCulture,
                $"Entity {c.Name} with ID={c.Id} consumes {c.DayConsumption} kWh per day");
        }

        return result.ToString();
    }
}