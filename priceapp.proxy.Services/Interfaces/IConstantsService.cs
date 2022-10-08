using priceapp.proxy.Models;

namespace priceapp.proxy.Services.Interfaces;

public interface IConstantsService
{
    Task<ConstantModel> GetConstantAsync(string label);
    Task SetConstantAsync(ConstantModel model);
}