FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["priceapp.Background/priceapp.Background.csproj", "priceapp.Background/"]
COPY ["priceapp.ControllersLogic/priceapp.ControllersLogic.csproj", "priceapp.ControllersLogic/"]
COPY ["priceapp.ShopsServices/priceapp.ShopsServices.csproj", "priceapp.ShopsServices/"]
COPY ["priceapp.Models/priceapp.Models.csproj", "priceapp.Models/"]
COPY ["priceapp.proxy.Models/priceapp.proxy.Models.csproj", "priceapp.proxy.Models/"]
COPY ["priceapp.proxy/priceapp.proxy.csproj", "priceapp.proxy/"]
COPY ["priceapp.proxy.Services/priceapp.proxy.Services.csproj", "priceapp.proxy.Services/"]
COPY ["priceapp.proxy.Repositories.Models/priceapp.proxy.Repositories.Models.csproj", "priceapp.proxy.Repositories.Models/"]
COPY ["priceapp.proxy.Repositories/priceapp.proxy.Repositories.csproj", "priceapp.proxy.Repositories/"]
COPY ["priceapp.Utils/priceapp.Utils.csproj", "priceapp.Utils/"]
COPY ["priceapp.Repositories/priceapp.Repositories.csproj", "priceapp.Repositories/"]
COPY ["priceapp.Repositories.Models/priceapp.Repositories.Models.csproj", "priceapp.Repositories.Models/"]
COPY ["priceapp.proxy.ShopServices/priceapp.proxy.ShopServices.csproj", "priceapp.proxy.ShopServices/"]
COPY ["priceapp.tasks/priceapp.tasks.csproj", "priceapp.tasks/"]
COPY ["priceapp.Services/priceapp.Services.csproj", "priceapp.Services/"]
COPY ["priceapp.ShopsServices.Models/priceapp.ShopsServices.Models.csproj", "priceapp.ShopsServices.Models/"]
RUN dotnet restore "priceapp.Background/priceapp.Background.csproj"
COPY . .
WORKDIR "/src/priceapp.Background"
RUN dotnet build "./priceapp.Background.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./priceapp.Background.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "priceapp.Background.dll"]
