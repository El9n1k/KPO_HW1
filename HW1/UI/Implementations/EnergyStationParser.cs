using HW1.Models;
using HW1.Models.Implementations;

namespace HW1.UI.Implementations;

public class EnergyStationParser : IEntityParser
{
    public string EntityType { get; } = "EnergyStation";

    public IEntity Parse()
    {
        return new EnergyStation
        {
            Name = ConsoleInput.ReadNonEmpty("Название: "),
            Simplicity = ConsoleInput.ReadInt("Простота (1-10): ", 1, 10),
            DayConsumption = ConsoleInput.ReadDouble("Потребление в день (Квт/ч): ", 0)
        };
    }
}