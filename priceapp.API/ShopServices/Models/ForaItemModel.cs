namespace priceapp.API.ShopServices.Models;

public class ForaItemModel
{
    public List<object> sets { get; set; }
    public List<ForaItemCategories> categories { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string unit { get; set; }
    public double price { get; set; }
    public double? oldPrice { get; set; }
    public string mainImage { get; set; }
    public double? division { get; set; }
    public ForaPromotion? promotion { get; set; }
    public string slug { get; set; }
    public double? storeQuantity { get; set; }
    public string? promoId { get; set; }
    public string? promoTitle { get; set; }
    public List<ForaBubble> bubbles { get; set; }
    public string? priceStartFrom { get; set; }
    public string? priceStopAfter { get; set; }
    public double? calcStoreQuantity { get; set; }
    public bool? ecoPacking { get; set; }
    public string? recommendationBlockName { get; set; }
    public object? facings { get; set; }
    public List<ForaItemParameters> parameters { get; set; }
    public List<ForaItemPrices> prices { get; set; }
    public List<ForaPromotion> promotions { get; set; }
    public double? rating { get; set; }
    public int? votesCount { get; set; }
    public object? complexSku { get; set; }
    public double? quantity { get; set; }
    public string? highlight { get; set; }
    public object? techParameters { get; set; }
}

public class ForaBubble
{
    public string bubblePath { get; set; }
    public string type { get; set; }
    public string id { get; set; }
}

public class ForaPromotion
{
    public string id { get; set; }
    public string title { get; set; }
    public string iconPath { get; set; }
    public List<object> items { get; set; }
    public string startFrom { get; set; }
    public string stopAfter { get; set; }
    public string description { get; set; }
    public double? bonusQuantity { get; set; }
    public string? backColor { get; set; }
    public double? minCount { get; set; }
    public string? fontColor { get; set; }
}

public class ForaCatalogItems
{
    public List<ForaItemModel> items { get; set; }
    public int itemsCount { get; set; }
    public object EComError { get; set; }
    public object filials { get; set; }
    public object correction { get; set; }
}

public class ForaItemParameters
{
    public string key { get; set; }
    public string value { get; set; }
    public string name { get; set; }
    public int? valueId { get; set; }
}

public class ForaItemPrices
{
    public string Type { get; set; }
    public double Value { get; set; }
}

public class ForaItemCategories
{
    public int id { get; set; }
    public int? parentId { get; set; }
    public string? name { get; set; }
    public string? iconPath { get; set; }
    public int? order { get; set; }
    public string? mobileAppIconPath { get; set; }
    public string? slug { get; set; }
}