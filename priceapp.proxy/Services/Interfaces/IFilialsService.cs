using priceapp.proxy.Services.Models;

namespace priceapp.proxy.Services.Interfaces;

public interface IFilialsService
{
    Task<List<AtbFilialModel>> GetAtbFilialsAsync();
    Task InsertAsync(List<AtbFilialModel> models);
}