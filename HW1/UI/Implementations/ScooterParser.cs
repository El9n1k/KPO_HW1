using HW1.Models;
using HW1.Models.Implementations;

namespace HW1.UI.Implementations;

public class ScooterParser : IEntityParser
{
    public string EntityType { get; } = "Scooter";

    public IEntity Parse()
    {
        return new Scooter
        {
            Name = ConsoleInput.ReadNonEmpty("Название: "),
            Simplicity = ConsoleInput.ReadInt("Простота (1-10): ", 1, 10),
            MaxSpeed = ConsoleInput.ReadInt("Максимальная скорость, км/ч: ", 1, 100)
        };
    }
}