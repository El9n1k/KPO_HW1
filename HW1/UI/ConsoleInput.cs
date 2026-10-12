using System.Globalization;

namespace HW1.UI;

internal static class ConsoleInput
{
    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value)
                && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Введите целое число от {min} до {max}.");
        }
    }

    public static string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }

            Console.WriteLine("Значение не может быть пустым.");
        }
    }

    public static double ReadDouble(string prompt, double min)
    {
        while (true)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine()?.Replace(',', '.');

            if (double.TryParse(text, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out double value)
                && value >= min)
            {
                return value;
            }

            Console.WriteLine("Введите неотрицательное число (например, 1.5).");
        }
    }
}