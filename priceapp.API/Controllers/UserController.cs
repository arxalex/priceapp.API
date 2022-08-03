using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Request;
using priceapp.API.Controllers.Models.Response;
using priceapp.API.Models;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUsersService _usersService;

    public UserController(IUsersService usersService, ITokenService tokenService)
    {
        _usersService = usersService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel model)
    {
        try
        {
            int expires;
            string token;
            UserModel user;
            if (model.Username.Contains('@'))
                (user, token, expires) =
                    await _usersService.GetUserAndTokenByEmailAsync(model.Username, model.Password);
            else
                (user, token, expires) =
                    await _usersService.GetUserAndTokenByUsernameAsync(model.Username, model.Password);

            await _tokenService.InsertTokenAsync(user.Id, token, expires);

            var response = new LoginResponseModel
            {
                Role = user.Role,
                Status = true,
                Token = token
            };
            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new ErrorResponseModel {Status = false, Message = e.Message, Code = "WUL1"};
            return BadRequest(response);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestModel model)
    {
        try
        {
            await _usersService.RegisterUserAsync(model.Username, model.Email, model.Password);
            var response = new SuccessResponseModel {Status = true};
            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new ErrorResponseModel {Status = false, Message = e.Message, Code = "WUR1"};
            return BadRequest(response);
        }
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmailAsync([FromBody] VerifyEmailRequestModel model)
    {
        try
        {
            if ((await _usersService.GetUserByIdAsync(model.UserId)).Role != 0)
                return Ok(new SuccessResponseModel {Status = false, Message = "Already verified", Code = "IUVe2"});

            await _usersService.VerifyUserEmailAsync(model.UserId, model.Token);
            return Ok(new SuccessResponseModel {Status = true, Message = "Verification successful", Code = "IUVe1"});
        }
        catch (Exception e)
        {
            return BadRequest(new ErrorResponseModel {Status = false, Message = e.Message, Code = "WUVe3"});
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequestModel model)
    {
        try
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.Sid).Value);
            await _usersService.ChangePasswordAsync(userId, model.Password, model.PasswordOld);
            var (user, token, expires) = await _usersService.GetUserAndTokenAsync(userId, model.Password);

            await _tokenService.InsertTokenAsync(user.Id, token, expires);

            return Ok(new LoginResponseModel
            {
                Role = user.Role,
                Status = true,
                Token = token
            });
        }
        catch (Exception e)
        {
            return BadRequest(new ErrorResponseModel {Status = false, Message = e.Message, Code = "WUCp1"});
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            await _tokenService.DeactivateTokenAsync();
            return Ok(new SuccessResponseModel {Status = true});
        }
        catch (Exception e)
        {
            return BadRequest(new ErrorResponseModel {Status = false, Message = e.Message, Code = "WUL1"});
        }
    }
}