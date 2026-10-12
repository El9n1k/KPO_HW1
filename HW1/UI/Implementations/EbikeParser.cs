using HW1.Models;
using HW1.Models.Implementations;

namespace HW1.UI.Implementations;

public class EbikeParser : IEntityParser
{
    public string EntityType { get; } = "Ebike";

    public IEntity Parse()
    {
        return new EBike
        {
            Name = ConsoleInput.ReadNonEmpty("Название: "),
            Simplicity = ConsoleInput.ReadInt("Простота (1-10): ", 1, 10),
            MaxSpeed = ConsoleInput.ReadInt("Максимальная скорость, км/ч: ", 1, 100),
            DayConsumption = ConsoleInput.ReadDouble("Потребление энергии в день (Квт/ч): ", 0)
        };
    }
}