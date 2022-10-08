using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.Models;
using priceapp.Services.Interfaces;
using priceapp.ShopsServices.Interfaces;
using priceapp.tasks;
using RestSharp;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPricesService _pricesService;
    private readonly proxy.Controllers.PricesController _pricesController;
    private readonly ISilpoService _silpoService;
    private readonly IForaService _foraService;
    private readonly IAtbService _atbService;
    private readonly ICategoriesService _categoriesService;
    private readonly SessionParameters _sessionParameters;
    private readonly IFilialsService _filialsService;
    private readonly ThreadsUtil _threadsUtil;
    private readonly IConfiguration _configuration;
    private readonly RestClient _client;
    private readonly ITokenService _tokenService;

    public PricesController(IPricesService pricesService, proxy.Controllers.PricesController pricesController,
        ISilpoService silpoService, IForaService foraService, IAtbService atbService,
        ICategoriesService categoriesService, SessionParameters sessionParameters, IFilialsService filialsService,
        ThreadsUtil threadsUtil, IConfiguration configuration, ITokenService tokenService)
    {
        _pricesService = pricesService;
        _pricesController = pricesController;
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _categoriesService = categoriesService;
        _sessionParameters = sessionParameters;
        _filialsService = filialsService;
        _threadsUtil = threadsUtil;
        _configuration = configuration;
        _tokenService = tokenService;
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{_configuration["Domain:ProxyApi"]}/")
        };

        _client = new RestClient(httpClient);
    }

    [HttpPost("actualize")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizePricesAsync()
    {
        if (bool.Parse(_configuration["Threads:UseMultiThreading"]))
        {
            await StartUpdatePricesTasksAsync();
            return Ok();
        }

        await UpdatePricesAsync();
        return Ok();
    }

    [HttpPost("actualize/proxy/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeProxyPricesAsync([FromRoute] int shopId)
    {
        if (bool.Parse(_configuration["Proxy:MultiInstance"]))
        {
            await StartUpdateProxyPricesAsync(shopId);
            return Ok();
        }

        await _pricesController.ActualizeProxyPricesAsync(shopId);

        return Ok();
    }

    [HttpPost("refactor")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> RefactorPricesAsync()
    {
        await _pricesService.RefactorPricesAsync();
        return Ok();
    }

    private async Task<List<PriceModel>> GetPricesAsync(int shopId, int internalFilialId, int categoryId)
    {
        var prices = shopId switch
        {
            1 => await _silpoService.GetPricesAsync(categoryId, internalFilialId),
            2 => await _foraService.GetPricesAsync(categoryId, internalFilialId),
            3 => await _atbService.GetPricesAsync(categoryId, internalFilialId),
            _ => new List<PriceModel>()
        };

        return prices;
    }

    private async Task UpdatePricesAsync(FilialModel filial)
    {
        var categories = await _categoriesService.GetBaseCategoriesAsync();
        var prices = new List<PriceModel>();
        foreach (var category in categories)
        {
            prices.AddRange(await GetPricesAsync(filial.ShopId, filial.InShopId, category.Id));
        }

        var pricesHistory = prices.Select(x => new PriceHistoryModel()
        {
            Id = -1,
            Date = DateTime.Now,
            FilialId = x.FilialId,
            ItemId = x.ItemId,
            Price = x.Price,
            ShopId = x.ShopId
        }).ToList();

        await _pricesService.InsertOrUpdatePricesAsync(prices);
        await _pricesService.InsertOrUpdatePricesHistoryAsync(pricesHistory);
    }

    private async Task StartUpdatePricesTasksAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false)
    {
        if (_sessionParameters.IsActualizePricesActive)
        {
            return;
        }

        _sessionParameters.IsActualizePricesActive = true;

        var lastFilial = forceUpdate ? 0 : await _pricesService.GetMaxFilialIdToday() ?? 0;
        var filials = forceUpdate
            ? await _filialsService.GetFilialsAsync()
            : (await _filialsService.GetFilialsAsync()).Where(x => x.Id >= lastFilial).ToList();

        foreach (var filial in filials)
        {
            async Task Action()
            {
                try
                {
                    if (!skipSetZeroQuantity)
                    {
                        await _pricesService.SetPriceQuantitiesZeroAsync(filial.Id);
                    }

                    await UpdatePricesAsync(filial);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            await _threadsUtil.AddTask(Action(), Priority.Medium);
        }

        await _threadsUtil.AddTask(new Task(() => _sessionParameters.IsActualizePricesActive = false), Priority.Medium);
    }

    private async Task UpdatePricesAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false)
    {
        if (_sessionParameters.IsActualizePricesActive)
        {
            return;
        }

        _sessionParameters.IsActualizePricesActive = true;

        var lastFilial = forceUpdate ? 0 : await _pricesService.GetMaxFilialIdToday() ?? 0;
        var filials = forceUpdate
            ? await _filialsService.GetFilialsAsync()
            : (await _filialsService.GetFilialsAsync()).Where(x => x.Id >= lastFilial).ToList();

        foreach (var filial in filials)
        {
            try
            {
                if (!skipSetZeroQuantity)
                {
                    await _pricesService.SetPriceQuantitiesZeroAsync(filial.Id);
                }

                await UpdatePricesAsync(filial);
            }
            catch (Exception)
            {
                _sessionParameters.IsActualizePricesActive = false;
                throw;
            }
        }

        _sessionParameters.IsActualizePricesActive = false;
    }

    private async Task StartUpdateProxyPricesAsync(int shopId)
    {
        var request = new RestRequest($"Prices/actualize/{shopId}", Method.Post);

        request.AddHeader("Authorization", $"Bearer {_tokenService.GetCurrentAsync()}");
        _client.Execute(request);
    }
}