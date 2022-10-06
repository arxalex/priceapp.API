using System.Globalization;

namespace priceapp.Utils;

public static class NumericHelper
{
    public static (double?, string) ParseNumberString(string str)
    {
        const string numbers = "1234567890,.";
        var number = "";
        foreach (var ch in str)
            if (numbers.Contains(ch))
                number += ch != ',' ? ch : '.';
            else
                break;

        if (number.Length == 0) return (null, str);

        var label = str[number.Length..];

        return (double.Parse(number, CultureInfo.InvariantCulture), label);
    }
}