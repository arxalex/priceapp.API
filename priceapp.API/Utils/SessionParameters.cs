namespace priceapp.API.Utils;

public class SessionParameters
{
    private readonly proxy.Utils.SessionParameters _sessionParameters;
    public SessionParameters(proxy.Utils.SessionParameters sessionParameters)
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