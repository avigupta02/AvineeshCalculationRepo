using TwoNumberAddition;

var parser = new InputParser();
var calculator = new AdditionCalculator();

Console.Write("Number 1: ");
var firstInput = Console.ReadLine();
Console.Write("Number 2: ");
var secondInput = Console.ReadLine();

if (!parser.TryParse(firstInput, out var first) || !parser.TryParse(secondInput, out var second))
{
    Console.WriteLine("Error: please provide two valid numeric values.");
    return;
}

var sum = calculator.Calculate(first, second);
Console.WriteLine($"Sum: {sum.ToString(CultureInfo.CurrentCulture)}");
