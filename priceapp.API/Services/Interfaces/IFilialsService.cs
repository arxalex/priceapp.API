using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IFilialsService
{
    Task<List<FilialModel>> GetFilialsByLocationAsync(double xCord, double yCord, double radius);
}