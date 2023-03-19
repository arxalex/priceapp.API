using priceapp.Models;
using priceapp.Models.Enums;
using priceapp.Services.Interfaces;
using priceapp.Utils;

namespace priceapp.Services.Implementation;

public class ShoppingListService : IShoppingListService
{
    private readonly IPricesService _pricesService;

    public ShoppingListService(IPricesService pricesService)
    {
        _pricesService = pricesService;
    }

    public async Task<(List<PriceModel>, double)> ProcessShoppingList(CartProcessingType type,
        List<ShoppingListModel> items,
        double xCord, double yCord, double radius)
    {
        var itemsFromFilials = await _pricesService.GetPricesAsync(items.Select(x => x.ItemId), xCord, yCord, radius);
        List<PriceModel> result;
        switch (type)
        {
            case CartProcessingType.OneMarket:
                throw new ArgumentOutOfRangeException("filialId", null, null);
            case CartProcessingType.OneMarketLowest:
                result = GetPricesOneMarketLowest(items, itemsFromFilials);
                break;
            case CartProcessingType.MultipleMarketsLowest:
                result = GetPricesMultipleMarkets(items, itemsFromFilials);
                break;
            case CartProcessingType.MultipleMarketsInUse:
                result = GetPricesMultipleMarketsInUse(items, itemsFromFilials);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (result.Count != items.Count)
        {
            throw new Exception("Cant find all items from shopping list");
        }

        var maxPriceModels = itemsFromFilials
            .GroupBy(x => x.ItemId)
            .Where(x => result.Any(y => y.ItemId == x.Key))
            .Select(x => x.MaxBy(y => y.PriceFinal)!)
            .ToList();

        var economy = GetEconomy(items, result, maxPriceModels);

        return (result, economy);
    }

    private static double GetEconomy(IReadOnlyCollection<ShoppingListModel> shoppingList,
        IEnumerable<PriceModel> minPrices,
        IEnumerable<PriceModel> maxPrices)
    {
        return maxPrices
                   .Sum(x => x.PriceFinal * shoppingList
                       .First(y => y.ItemId == x.ItemId).Count) -
               minPrices
                   .Sum(x => x.PriceFinal * shoppingList
                       .First(y => y.ItemId == x.ItemId).Count);
    }

    private static List<PriceModel> GetPricesOneMarket(IReadOnlyCollection<ShoppingListModel> shoppingList,
        IEnumerable<PriceModel> priceModels, int filialId)
    {
        var pricesModelsCopy = priceModels.ToList();
        var result = new List<PriceModel>();

        var fixedPrices = pricesModelsCopy
            .Where(x =>
                shoppingList.Any(y => y.FilialId != null && y.ItemId == x.ItemId && y.FilialId == x.FilialId));
        pricesModelsCopy.RemoveAll(x => fixedPrices.Any(y => y.ItemId == x.ItemId));
        result.AddRange(fixedPrices);

        var prices = pricesModelsCopy
            .Where(x => x.FilialId == filialId);

        result.AddRange(prices);

        return result;
    }

    private static List<PriceModel> GetPricesOneMarketLowest(IReadOnlyCollection<ShoppingListModel> shoppingList,
        IEnumerable<PriceModel> priceModels)
    {
        var pricesModelsCopy = priceModels.ToList();
        var result = new List<PriceModel>();

        var fixedPrices = pricesModelsCopy
            .Where(x =>
                shoppingList.Any(y => y.FilialId != null && y.ItemId == x.ItemId && y.FilialId == x.FilialId));
        pricesModelsCopy.RemoveAll(x => fixedPrices.Any(y => y.ItemId == x.ItemId));
        result.AddRange(fixedPrices);

        var minPrices = pricesModelsCopy
            .GroupBy(x => x.FilialId)
            .Where(x =>
            {
                var pricesList = x.Select(y => y.ItemId).ToList();
                var shoppingListList = shoppingList
                    .Where(y => y.FilialId == null)
                    .Select(y => y.ItemId).ToList();
                pricesList.Sort();
                shoppingListList.Sort();
                return pricesList.SequenceEqual(shoppingListList);
            })
            .MinBy(x =>
                x.Sum(y => y.PriceFinal * shoppingList
                    .First(z => z.ItemId == y.ItemId).Count))!;

        result.AddRange(minPrices);

        return result;
    }

    private static List<PriceModel> GetPricesMultipleMarkets(IReadOnlyCollection<ShoppingListModel> shoppingList,
        IEnumerable<PriceModel> priceModels)
    {
        var pricesModelsCopy = priceModels.ToList();
        var result = new List<PriceModel>();

        var fixedPrices = pricesModelsCopy
            .Where(x =>
                shoppingList.Any(y => y.FilialId != null && y.ItemId == x.ItemId && y.FilialId == x.FilialId));
        pricesModelsCopy.RemoveAll(x => fixedPrices.Any(y => y.ItemId == x.ItemId));

        var itemsMinPrices = pricesModelsCopy
            .GroupBy(x => x.ItemId)
            .SelectMany(x =>
                x.Where(y => Math.Abs(y.PriceFinal - x.Min(z => z.PriceFinal)) < NumericHelper.ToleranceUAH))
            .ToList();

        var uniqueItems = itemsMinPrices
            .GroupBy(x => x.ItemId)
            .Where(x => x.Count() == 1)
            .SelectMany(x => x)
            .ToList();
        uniqueItems.AddRange(fixedPrices);
        result.AddRange(uniqueItems);

        var duplicateItems = itemsMinPrices
            .Where(x => result.All(y => y.ItemId != x.ItemId))
            .ToList();

        if (duplicateItems.Count <= 0)
        {
            return result;
        }

        var filialsAlreadyInUse = uniqueItems.Select(x => x.FilialId).Distinct();

        var duplicateAlreadyInFilials = duplicateItems
            .Where(x => filialsAlreadyInUse.Any(y => y == x.FilialId))
            .DistinctBy(x => x.ItemId)
            .ToList();

        result.AddRange(duplicateAlreadyInFilials);

        var rest = duplicateItems
            .Where(x => duplicateAlreadyInFilials.All(y => y.ItemId != x.ItemId))
            .ToList();

        if (rest.Count <= 0)
        {
            return result;
        }

        result.AddRange(GetRestItems(rest));

        return result;
    }

    private static List<PriceModel> GetPricesMultipleMarketsInUse(IReadOnlyCollection<ShoppingListModel> shoppingList,
        IEnumerable<PriceModel> priceModels)
    {
        var pricesModelsCopy = priceModels.ToList();
        var result = new List<PriceModel>();

        var fixedPrices = pricesModelsCopy
            .Where(x =>
                shoppingList.Any(y => y.FilialId != null && y.ItemId == x.ItemId && y.FilialId == x.FilialId))
            .ToList();
        pricesModelsCopy.RemoveAll(x => fixedPrices.Any(y => y.ItemId == x.ItemId));
        result.AddRange(fixedPrices);

        var pricesMinFromExistedFilials = pricesModelsCopy
            .Where(x => fixedPrices.Any(y => y.FilialId == x.FilialId))
            .GroupBy(x => x.ItemId)
            .Select(x => x.MinBy(y => y.PriceFactor)!);

        result.AddRange(pricesMinFromExistedFilials);

        return result;
    }

    private static IEnumerable<PriceModel> GetRestItems(IReadOnlyCollection<PriceModel> rest)
    {
        var filialsAfter = rest
            .Select(x => x.FilialId)
            .Distinct()
            .ToList();
        var itemsAfter = rest
            .Select(x => x.ItemId)
            .Distinct()
            .ToList();
        var totalItems = itemsAfter.Count;
        var totalFilials = filialsAfter.Count;
        var totalCycles = Math.Min(totalItems, totalFilials);
        return NumericHelper.Binomial(filialsAfter.Count, totalCycles) < 2048
            ? GetViaCombination(rest, totalCycles, filialsAfter)
            : GetViaGrouping(rest);
    }

    private static List<PriceModel> GetViaGrouping(IEnumerable<PriceModel> rest)
    {
        var restCopy = rest.ToList();
        var result = new List<PriceModel>();
        while (restCopy.Count > 0)
        {
            var itemsByFilial = restCopy.GroupBy(x => x.FilialId).ToList();
            var maxItems = itemsByFilial
                .First(x => x.Count() == itemsByFilial.Max(y => y.Count()))
                .ToList();
            result.AddRange(maxItems);
            restCopy.RemoveAll(x => maxItems.Any(y => y.ItemId == x.ItemId));
        }

        return result;
    }

    private static List<PriceModel> GetViaCombination(IReadOnlyCollection<PriceModel> rest, int totalCycles,
        List<int> filials)
    {
        for (var i = 1; i <= totalCycles; i++)
        {
            var filialsCombination = NumericHelper.GenerateCombinations(filials.ToArray(), i);
            foreach (var combination in filialsCombination)
            {
                var final = new List<PriceModel>();
                var restCopy = rest.ToList();
                foreach (var filialId in combination)
                {
                    var itemsFromThisFilial = restCopy.Where(x => x.FilialId == filialId);
                    restCopy.RemoveAll(x => itemsFromThisFilial.Any(y => y.ItemId == x.ItemId));
                    final.AddRange(itemsFromThisFilial);
                    if (restCopy.Count < 0)
                    {
                        return final;
                    }
                }
            }
        }

        return new List<PriceModel>();
    }
}