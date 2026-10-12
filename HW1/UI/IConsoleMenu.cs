namespace HW1.UI;

public enum CommandType { Add, Quit, Report, Remove, Unknown, Demo }

public interface IConsoleMenu
{
    bool ProcessCommand(); //false - завершение программы
    void PrintHelp();
}