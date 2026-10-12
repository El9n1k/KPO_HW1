using HW1.Models;
using HW1.Models.Implementations;

namespace HW1.UI.Implementations;

public class HelmetParser : IEntityParser
{
    public string EntityType { get; } = "Helmet";

    public IEntity Parse()
    {
        return new Helmet
        {
            Name = ConsoleInput.ReadNonEmpty("Название: "),
            Simplicity = ConsoleInput.ReadInt("Простота (1-10): ", 1, 10)
        };
    }
}