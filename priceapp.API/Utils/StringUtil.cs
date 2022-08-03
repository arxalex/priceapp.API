using System.Text.RegularExpressions;

namespace priceapp.API.Utils;

public static class StringUtil
{
    public static string QuotesReplacer(string str)
    {
        var replaced = ReplaceAll(str,
            new[] {'®', '™', '©'},
            '\0'
        );
        replaced = ReplaceAll(replaced,
            new[]
            {
                '‘', '’', '‚', '‛', '❛', '❜', '＇'
            },
            '\''
        );
        replaced = ReplaceAll(replaced, new[]
            {
                '«', '»', '‹', '›', '“', '”', '„', '‟', '❝', '❞', '〝', '〞', '〟', '＂'
            },
            '\"');
        return replaced;
    }

    public static List<string> NameToKeywords(string str)
    {
        var replaced = QuotesReplacer(str);
        var replacedWithoutQuotes = replaced.Replace('\"', '\0');
        var preResult = replacedWithoutQuotes.Split(' ');
        var result = preResult.Where(value => value != "").ToList();
        var trimResult = result.Select(value => value[..^1]).ToList();

        result.AddRange(trimResult);

        return result;
    }

    public static string ReplaceAll(string seed, IEnumerable<char> chars, char replacementCharacter)
    {
        return chars.Aggregate(seed, (str, cItem) => str.Replace(cItem, replacementCharacter));
    }

    public static string StringCleaner(string str)
    {
        var reg = new Regex(@"~[^\p{Cyrillic}a-z0-9_\s-]+~ui");
        return reg.Replace(str, "");
    }

    public static Dictionary<int, int> RateItemsByKeywords(string label, List<string> itemLabels)
    {
        var keywords = NameToKeywords(StringCleaner(label));
        var rates = new Dictionary<int, int>(itemLabels.Count);
        for (var key = 0; key < itemLabels.Count; key++)
        {
            var itemLabel = itemLabels[key];
            var itemKeywords = NameToKeywords(StringCleaner(itemLabel));
            var tempRate = 0;
            tempRate += itemLabel == label ? keywords.Count * 8 : 0;
            tempRate += itemLabel.Contains(label) ? keywords.Count * 4 : 0;
            tempRate += label.Contains(itemLabel) ? itemKeywords.Count * 4 : 0;
            var i = 0;
            var maxI = keywords.Count;
            foreach (var unused in itemKeywords.Where(itemStr => itemStr == keywords[i]))
            {
                tempRate += 2;
                i++;
                if (i == maxI) break;
            }

            i = 0;
            maxI = itemKeywords.Count;
            foreach (var unused in keywords.Where(labelStr => labelStr == itemKeywords[i]))
            {
                tempRate += 2;
                i++;
                if (i == maxI) break;
            }

            tempRate +=
                (from itemStr in itemKeywords from labelStr in keywords select labelStr == itemStr ? 1 : 0).Sum();
            rates[key] = tempRate;
        }

        return rates;
    }

    public static bool IsValidUsername(string username)
    {
        if (username.Length < 1) return false;

        const string availableCharString = "0123456789abcdefghijklmnopqrstuvwxyz_.";
        return username.All(character => availableCharString.Contains(character.ToString()));
    }

    public static bool IsValidEmail(string email)
    {
        if (email.Length < 5) return false;

        const string pattern =
            @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";

        var match = Regex.Match(email, pattern);

        return match.Success;
    }

    public static string GenerateRandomString(int length)
    {
        const string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var randomString = "";
        var random = new Random();
        for (var i = 0; i < length; i++) randomString += characters[random.Next(0, characters.Length - 1)];
        return randomString;
    }
}