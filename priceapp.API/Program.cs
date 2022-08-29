using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using priceapp.API.Repositories;
using priceapp.API.Repositories.Implementation;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Implementation;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Implementation;
using priceapp.API.ShopServices.Interfaces;
using priceapp.API.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["JWTSetting:SecretKey"];
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var tokenService = serviceProvider.GetService<ITokenService>();
            if (!tokenService!.IsCurrentTokenActive().Result) context.Fail("Unauthorized");

            return Task.CompletedTask;
        }
    };
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddSingleton(new MySQLDbConnectionFactory(builder.Configuration["ConnectionStrings:Default"]));
builder.Services.AddSingleton(new JWTSetting
{
    SecretKey = builder.Configuration["JWTSetting:SecretKey"],
    Audience = builder.Configuration["JWTSetting:Audience"],
    Issuer = builder.Configuration["JWTSetting:Issuer"],
    Lifetime = int.Parse(builder.Configuration["JWTSetting:Lifetime"])
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IBrandsService, BrandsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<ICategoryLinksService, CategoryLinksService>();
builder.Services.AddScoped<IConsistsService, ConsistsService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IFilialsService, FilialsService>();
builder.Services.AddScoped<IItemLinksService, ItemLinksService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IPackagesService, PackagesService>();
builder.Services.AddScoped<IPricesService, PricesService>();
builder.Services.AddScoped<IShopsService, ShopsService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddScoped<ISilpoService, SilpoService>();
builder.Services.AddScoped<IAtbService, AtbService>();
builder.Services.AddScoped<IForaService, ForaService>();

builder.Services.AddScoped<IBrandsRepository, BrandsRepository>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<ICategoryLinksRepository, CategoryLinksRepository>();
builder.Services.AddScoped<IConsistsRepository, ConsistsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IFilialsRepository, FilialsRepository>();
builder.Services.AddScoped<IItemsRepository, ItemsRepository>();
builder.Services.AddScoped<IPackagesRepository, PackagesRepository>();
builder.Services.AddScoped<IPricesRepository, PricesRepository>();
builder.Services.AddScoped<IShopsRepository, ShopsRepository>();
builder.Services.AddScoped<ITokensRepository, TokensRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CorsPolicy",
        corsPolicyBuilder => corsPolicyBuilder.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();