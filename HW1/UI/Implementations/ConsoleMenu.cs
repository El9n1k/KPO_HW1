using System.Text;

using HW1.Models;
using HW1.Services;

namespace HW1.UI.Implementations;

public class ConsoleMenu : IConsoleMenu
{
    private readonly DemoRunner _demo;
    private readonly Park _park;
    private readonly List<IEntityParser> _parsers;
    private readonly List<IReport> _reports;

    public ConsoleMenu(Park park, IEnumerable<IEntityParser> parsers,
        IEnumerable<IReport> reports, DemoRunner demo)
    {
        _park = park;
        _parsers = parsers.ToList();
        _reports = reports.ToList();
        _demo = demo;
    }

    public bool ProcessCommand()
    {
        CommandType command = ParseCommand();
        switch (command)
        {
            case CommandType.Quit:
                return false;
            case CommandType.Demo:
                _demo.Run();
                return true;
            case CommandType.Add:
                if (ParseEntity() < 0)
                {
                    PrintHelp();
                }

                return true;
            case CommandType.Remove:
                RemoveEntity();
                return true;
            case CommandType.Report:
                PrintReport();
                return true;
            case CommandType.Unknown:
            default:
                PrintHelp();
                return true;
        }
    }

    public void PrintHelp()
    {
        StringBuilder commands = new();
        foreach (CommandType command in Enum.GetValues<CommandType>())
        {
            if (command == CommandType.Unknown)
            {
                continue;
            }

            commands.Append(command);
            commands.Append(' ');
        }

        Console.WriteLine($"Команды: {commands}");
    }

    private static CommandType ParseCommand()
    {
        string? command = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (command == null)
        {
            return CommandType.Quit;
        }

        switch (command)
        {
            case "demo":
                return CommandType.Demo;
            case "add":
                return CommandType.Add;
            case "remove":
                return CommandType.Remove;
            case "report":
                return CommandType.Report;
            case "quit":
                return CommandType.Quit;
            default:
                return CommandType.Unknown;
        }
    }

    private int ParseEntity()
    {
        string possibleTypes = string.Join("/", _parsers.Select(p => p.EntityType));
        Console.Write($"Введите тип сущности {possibleTypes}: ");
        string? type = Console.ReadLine()?.Trim();

        foreach (IEntityParser parser in _parsers)
        {
            if (parser.EntityType.Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                IEntity entity = parser.Parse();
                if (_park.Add(entity))
                {
                    Console.WriteLine($"Ваша сущность добавлена, ID={entity.Id}");
                    return 0;
                }

                Console.WriteLine("Вашу сущность отклонили на тех осмотре");
                return 0;
            }
        }

        Console.WriteLine("Неизвестный тип сущности");
        return -1;
    }

    private void RemoveEntity()
    {
        int id = ConsoleInput.ReadInt("Введите ID сущности: ", 0, int.MaxValue);
        if (_park.DeleteById(id))
        {
            Console.WriteLine("Сущность удалена");
            return;
        }

        Console.WriteLine("Не получилось удалить сущность");
    }

    private void PrintReport()
    {
        string possibleTypes = string.Join("/", _reports.Select(p => p.Type));
        Console.Write($"Введите тип отчета {possibleTypes}: ");
        string? type = Console.ReadLine()?.Trim();
        foreach (IReport report in _reports)
        {
            if (report.Type.ToString().Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(report.CreateReport());
                return;
            }
        }

        Console.WriteLine("Неизвестный тип отчёта.");
    }
}