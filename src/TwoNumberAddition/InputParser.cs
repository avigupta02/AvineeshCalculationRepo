using System.Globalization;

namespace TwoNumberAddition;

public sealed class InputParser
{
    public bool TryParse(string? input, out decimal value) =>
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
}
