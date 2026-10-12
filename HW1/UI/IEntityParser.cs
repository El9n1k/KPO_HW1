using HW1.Models;

namespace HW1.UI;

public interface IEntityParser
{
    string EntityType { get; }
    IEntity Parse();
}