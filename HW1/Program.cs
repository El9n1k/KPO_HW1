using HW1.Services;
using HW1.Services.Implementations;
using HW1.UI;
using HW1.UI.Implementations;

using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();
services.AddSingleton<IServiceCenter, ServiceCenter>();
services.AddSingleton<IRepository, Repository>();
services.AddSingleton<IReport, EnergyReport>();
services.AddSingleton<IReport, NewbieReport>();
services.AddSingleton<IReport, EntitiesReport>();
services.AddSingleton<IEntityParser, BikeParser>();
services.AddSingleton<IEntityParser, EbikeParser>();
services.AddSingleton<IEntityParser, ScooterParser>();
services.AddSingleton<IEntityParser, HelmetParser>();
services.AddSingleton<IEntityParser, EnergyStationParser>();
services.AddSingleton<IInewbieRules, InewbieRules>();
services.AddSingleton<IConsoleMenu, ConsoleMenu>();
services.AddSingleton<DemoRunner>();
services.AddSingleton<Park>();

ServiceProvider provider = services.BuildServiceProvider();

IConsoleMenu consoleMenu = provider.GetRequiredService<IConsoleMenu>();

consoleMenu.PrintHelp();
while (consoleMenu.ProcessCommand()) { }