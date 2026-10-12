using HW1.Models;
using HW1.Models.Implementations;

namespace HW1.UI.Implementations;

public class BikeParser : IEntityParser
{
    public string EntityType { get; } = "Bike";

    public IEntity Parse()
    {
        return new Bike
        {
            Name = ConsoleInput.ReadNonEmpty("Название: "),
            Simplicity = ConsoleInput.ReadInt("Простота (1-10): ", 1, 10),
            MaxSpeed = ConsoleInput.ReadInt("Максимальная скорость, км/ч: ", 1, 100)
        };
    }
}