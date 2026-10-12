using HW1.Models;
using HW1.Models.Implementations;
using HW1.Services;

namespace HW1.UI.Implementations;

public class DemoRunner(Park park, IEnumerable<IReport> reports)
{
    private readonly Park _park = park;
    private readonly List<IReport> _reports = [.. reports];

    public void Run()
    {
        PrintTitle("1. Приём: каждый вид техники и вещей");
        Bike cityBike = new() { Name = "bike 1", Simplicity = 9, MaxSpeed = 25 };
        TryAdd(cityBike);
        TryAdd(new Scooter { Name = "scooter 1", Simplicity = 8, MaxSpeed = 25 });
        TryAdd(new EBike { Name = "Ebike 1", Simplicity = 4, MaxSpeed = 35, DayConsumption = 1.5 });
        TryAdd(new Helmet { Name = "Helmet 1", Simplicity = 9 });
        TryAdd(new EnergyStation { Name = "charging station 1", Simplicity = 5, DayConsumption = 12 });

        PrintTitle("2. Граница правила для новичков");
        TryAdd(new Bike { Name = "Higher", Simplicity = 7, MaxSpeed = 20 });
        TryAdd(new Bike { Name = "Borderline", Simplicity = 6, MaxSpeed = 20 });
        TryAdd(new Bike { Name = "Almost", Simplicity = 5, MaxSpeed = 20 });

        PrintTitle("3. Отказ техосмотра");
        TryAdd(new Scooter { Name = "MaxSpeed higher that limit", Simplicity = 7, MaxSpeed = 60 });
        TryAdd(new Bike { Name = "Broken simplicity", Simplicity = 11, MaxSpeed = 20 });
        TryAdd(new Helmet { Name = "No Rating", Simplicity = 0 });
        TryAdd(new EnergyStation { Name = "broken enery usage", Simplicity = 5, DayConsumption = -5 });

        PrintTitle("4. Отчёты");
        PrintReport(ReportType.Energy);
        PrintReport(ReportType.Newbie);
        PrintReport(ReportType.Entity);

        PrintTitle("5. Удаление и уникальность номеров");
        Remove(cityBike.Id);
        Remove(999);
        TryAdd(new Bike { Name = "Bike 2", Simplicity = 7, MaxSpeed = 20 });
        PrintReport(ReportType.Entity);
    }

    private void TryAdd(IEntity entity)
    {
        bool accepted = _park.Add(entity);
        Console.WriteLine(accepted
            ? $"Принято: {entity.Name}, инвентарный номер {entity.Id}"
            : $"Отклонено техосмотром: {entity.Name}");
    }

    private void Remove(int id)
    {
        Console.WriteLine(_park.DeleteById(id)
            ? $"Удалено: номер {id}"
            : $"Номер {id} не найден");
    }

    private void PrintReport(ReportType type)
    {
        Console.WriteLine(_reports.Single(r => r.Type == type).CreateReport());
    }

    private static void PrintTitle(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"    {title}    ");
    }
}