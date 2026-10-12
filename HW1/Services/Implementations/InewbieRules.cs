using HW1.Models;

namespace HW1.Services.Implementations;

public class InewbieRules : IInewbieRules
{
    private const int MinSimplicity = 6;

    public bool IsForNewbie(IEntity entity)
    {
        if (entity.Simplicity >= MinSimplicity) { return true; }

        // можно добавить другие условия тут или написать отедльный класс для более глубокой
        // проверки на то, что entity подходит новичку
        return false;
    }
}