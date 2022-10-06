namespace priceapp.API;

public class SessionParameters
{
    public SessionParameters()
    {
        IsActualizePricesActive = false;
        IsActualizeProxyAtbPricesActive = false;
    }

    public bool IsActualizePricesActive { get; set; }

    public bool IsActualizeProxyAtbPricesActive { get; set; }
}