using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace priceapp.ControllersLogic;

public class SessionParameters
{
    private readonly proxy.SessionParameters _sessionParameters;
    private readonly IConfiguration _configuration;
    private readonly RestClient _client;
    private bool _isActualizePricesActive;
    public SessionParameters(proxy.SessionParameters sessionParameters, IConfiguration configuration)
    {
        _sessionParameters = sessionParameters;
        _configuration = configuration;
        IsActualizePricesActive = false;
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{_configuration["Domain:Background"]}/")
        };

        _client = new RestClient(httpClient);
    }

    public bool IsActualizePricesActive
    {
        get
        {
            if (!bool.Parse(_configuration["Proxy:MultiInstance"]) || bool.Parse(_configuration["Proxy:IsBackground"]))
            {
                return _isActualizePricesActive;
            }

            try
            {
                var request = new RestRequest("Info/status/actualize");

                var response = _client.ExecuteAsync(request).Result;

                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                    throw new ConnectionAbortedException("Could not get data from Atb");

                var result = bool.Parse(response.Content);

                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
        set => _isActualizePricesActive = value;
    }

    public bool IsActualizeProxyAtbPricesActive
    {
        get => _sessionParameters.IsActualizeProxyAtbPricesActive;
        set => _sessionParameters.IsActualizeProxyAtbPricesActive = value;
    }
}