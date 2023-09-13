using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Response;
using priceapp.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ISystemService _systemService;
    private readonly proxy.Services.Interfaces.ISystemService _systemServiceProxy;

    public InfoController(IConfiguration configuration, ISystemService systemService,
        proxy.Services.Interfaces.ISystemService systemServiceProxy)
    {
        _configuration = configuration;
        _systemService = systemService;
        _systemServiceProxy = systemServiceProxy;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetVersion()
    {
        return Ok(new
        {
            Name = "Priceapp.API",
            Version = "1.1"
        });
    }

    [HttpGet("connection")]
    public async Task<IActionResult> TestConnection()
    {
        return Ok(new
        {
            DbMain = await _systemService.IsDbConnected() ? "Ok" : "Failed",
            DBProxy = await _systemServiceProxy.IsDbConnected() ? "Ok" : "Failed"
        });
    }

    [HttpGet("cores")]
    public async Task<IActionResult> GetCores()
    {
        return Ok(new
        {
            Count = bool.Parse(_configuration["Threads:UseSystem"])
                ? Environment.ProcessorCount
                : int.Parse(_configuration["Threads:DefaultCount"]),
            UseSystem = bool.Parse(_configuration["Threads:UseSystem"]),
            SystemCores = Environment.ProcessorCount
        });
    }

    [HttpGet("authorize-check")]
    [Authorize]
    public async Task<IActionResult> CheckAuthorize()
    {
        return Ok();
    }

    [HttpGet("update")]
    public async Task<IActionResult> CheckUpdate([FromQuery] string version)
    {
        try
        {
            return Ok(await _systemService.IsNeedUpdate(version));
        }
        catch (FormatException e)
        {
            return BadRequest(new ErrorResponseModel {Status = false, Message = e.Message, Code = "IIU1"});
        }
    }
}