using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Services.Models;

namespace priceapp.proxy.Repositories.Interfaces;

public interface IFilialsRepository
{
    Task<List<AtbFilialRepositoryModel>> GetAtbFilialsAsync();
    Task InsertOrUpdateAsync(List<AtbFilialRepositoryModel> models);
}