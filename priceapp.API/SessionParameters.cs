namespace priceapp.API;

public class SessionParameters
{
    private readonly proxy.SessionParameters _sessionParameters;
    public SessionParameters(proxy.SessionParameters sessionParameters)
    {
        _sessionParameters = sessionParameters;
        IsActualizePricesActive = false;
    }

    public bool IsActualizePricesActive { get; set; }

    public bool IsActualizeProxyAtbPricesActive
    {
        get => _sessionParameters.IsActualizeProxyAtbPricesActive;
        set => _sessionParameters.IsActualizeProxyAtbPricesActive = value;
    }
}