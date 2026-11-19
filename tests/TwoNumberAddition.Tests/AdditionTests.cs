using TwoNumberAddition;

namespace TwoNumberAddition.Tests;

public sealed class AdditionTests
{
    private readonly AdditionCalculator _calculator = new();
    private readonly InputParser _parser = new();

    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(-2, 3, 1)]
    [InlineData(1.25, 2.5, 3.75)]
    public void Calculate_returns_sum(decimal first, decimal second, decimal expected) =>
        Assert.Equal(expected, _calculator.Calculate(first, second));

    [Theory]
    [InlineData("12.5")]
    [InlineData("-3")]
    public void TryParse_accepts_numeric_input(string input) =>
        Assert.True(_parser.TryParse(input, out _));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-number")]
    public void TryParse_rejects_missing_or_non_numeric_input(string? input) =>
        Assert.False(_parser.TryParse(input, out _));
}
