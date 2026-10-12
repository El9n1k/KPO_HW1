namespace HW1.Models.Implementations;

public class Bike : IMovable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Simplicity { get; set; }
    public int MaxSpeed { get; set; }

    //тут могут быть прочие свойства или функции, релевантные только для велосипеда
}