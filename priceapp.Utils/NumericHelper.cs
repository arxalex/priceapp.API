using System.Globalization;

namespace priceapp.Utils;

public static class NumericHelper
{
    public const double ToleranceUAH = 0.01;
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
    
    public static int Binomial(int n, int k) {
        if (k > n - k) {
            k = n - k;
        }

        int result = 1;

        for (int i = 0; i < k; i++) {
            result *= n - i;
            result /= i + 1;
        }

        return result;
    }
    
    public static int[][] GenerateCombinations(int[] numbers, int k) {
        int[][] result = new int[Binomial(numbers.Length, k)][];

        int[] combination = new int[k];
        int index = 0;

        GenerateCombinationsHelper(numbers, combination, 0, 0, result, ref index);

        return result;
    }

    private static void GenerateCombinationsHelper(int[] numbers, int[] combination, int startIndex, int currentIndex, int[][] result, ref int index) {
        if (currentIndex == combination.Length) {
            result[index] = (combination.Clone() as int[])!; 
            index++;
            return;
        }

        if (startIndex == numbers.Length) {
            return;
        }

        for (int i = startIndex; i < numbers.Length; i++) {
            combination[currentIndex] = numbers[i];
            GenerateCombinationsHelper(numbers, combination, i + 1, currentIndex + 1, result, ref index);
        }
    }
}