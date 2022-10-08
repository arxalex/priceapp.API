using priceapp.proxy.Repositories.Models;

namespace priceapp.proxy.Repositories.Interfaces;

public interface IConstantsRepository
{
    Task<ConstantRepositoryModel> GetConstantAsync(string label);
    Task InsertOrUpdateConstantAsync(ConstantRepositoryModel model);
}