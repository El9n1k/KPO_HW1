namespace HW1.Models.Implementations;

public class EnergyStation : IEnergyConsumer
{
    public double DayConsumption { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Simplicity { get; set; }

    //тут могут быть прочие свойства или функции, релевантные только для зарядной станции
}