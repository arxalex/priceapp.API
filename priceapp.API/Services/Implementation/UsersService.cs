using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Interfaces;
using priceapp.API.Utils;

namespace priceapp.API.Services.Implementation;

public class UsersService : IUsersService
{
    private readonly JWTSetting _jwtSetting;
    private readonly ILogger<UsersService> _logger;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly ITokensRepository _tokensRepository;
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository, IMapper mapper, ILogger<UsersService> logger,
        IMailService mailService,
        ITokensRepository tokensRepository, JWTSetting jwtSetting, ITokenService tokenService)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
        _logger = logger;
        _mailService = mailService;
        _tokensRepository = tokensRepository;
        _jwtSetting = jwtSetting;
        _tokenService = tokenService;
    }

    public async Task<(UserModel, string token, int expires)> GetUserAndTokenByEmailAsync(string email, string password)
    {
        if (password.Length < 1 && !StringUtil.IsValidEmail(email))
        {
            _logger.LogInformation($"UserService: User with email {email} try to login with invalid arguments");
            throw new ArgumentException("Email or password invalid");
        }

        var user = _mapper.Map<UserModel>(await _usersRepository.GetUserByEmailAsync(email));

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogWarning($"UserService: User with username {user.Username} try to login with invalid password");
            throw new ArgumentException("Password is incorrect");
        }

        var identity = GetClaimIdentity(user);

        var (token, expires) = GenerateToken(identity);

        return (user, token, expires);
    }

    public async Task<(UserModel, string token, int expires)> GetUserAndTokenByUsernameAsync(string username,
        string password)
    {
        if (password.Length < 1 && !StringUtil.IsValidUsername(username))
        {
            _logger.LogInformation($"UserService: User with username {username} try to login with invalid arguments");
            throw new ArgumentException("Username or password invalid");
        }

        var user = _mapper.Map<UserModel>(await _usersRepository.GetUserByUsernameAsync(username));

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogWarning($"UserService: User with username {user.Username} try to login with invalid password");
            throw new ArgumentException("Password is incorrect");
        }

        var identity = GetClaimIdentity(user);

        var (token, expires) = GenerateToken(identity);

        return (user, token, expires);
    }

    public async Task<(UserModel, string token, int expires)> GetUserAndTokenAsync(int userId, string password)
    {
        if (password.Length < 1)
        {
            _logger.LogInformation($"UserService: User {userId} try to login with invalid arguments");
            throw new ArgumentException("UserId or password invalid");
        }

        var user = _mapper.Map<UserModel>(await _usersRepository.GetUserByIdAsync(userId));

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogWarning($"UserService: User with username {user.Username} try to login with invalid password");
            throw new ArgumentException("Password is incorrect");
        }

        var identity = GetClaimIdentity(user);

        var (token, expires) = GenerateToken(identity);

        return (user, token, expires);
    }

    public async Task RegisterUserAsync(string username, string email, string password)
    {
        if (password.Length < 1 && !StringUtil.IsValidEmail(email) && !StringUtil.IsValidUsername(username))
        {
            _logger.LogInformation(
                $"UserService: User with username {username} and email {email} try to register with invalid arguments");
            throw new ArgumentException("Username, email or password invalid");
        }

        if (await _usersRepository.IsUserExistsAsync(username, email))
        {
            _logger.LogInformation(
                $"UserService: User with username {username} and email {email} try to register with user that already exists");
            throw new InvalidDataException($"User with username {username} or email {email} already exists");
        }

        await _usersRepository.RegisterUserAsync(username, email, password, 0);
        var user = _mapper.Map<UserModel>(await _usersRepository.GetUserByUsernameAsync(username));
        var token = await CreateEmailConfirmTokenAsync(user.Id);

        await _mailService.SendRegistrationConfirmEmailAsync(user.Id, user.Email, token);

        _logger.LogInformation(
            $"UserService: User with username {username} and email {email} registered succesfull");
    }

    public async Task<UserModel> GetUserByIdAsync(int userId)
    {
        return _mapper.Map<UserModel>(await _usersRepository.GetUserByIdAsync(userId));
    }

    public async Task VerifyUserEmailAsync(int userId, string token)
    {
        if (!await _tokensRepository.IsConfirmEmailTokenExistsAsync(userId))
        {
            _logger.LogInformation($"UserService: Confirmation token does not exist for user {userId}");
            throw new VerificationException("Confirmation token does not exist");
        }

        await _tokensRepository.CloseConfirmEmailTokenAsync(userId, token);
        await _usersRepository.UpdateUserRole(userId, 1);
    }

    public async Task ChangePasswordAsync(int userId, string password, string passwordOld)
    {
        if (password.Length < 1 && passwordOld.Length < 1 && password != passwordOld)
        {
            _logger.LogInformation(
                $"UserService: User {userId} try to change password with invalid arguments");
            throw new ArgumentException("Old or new password is invalid");
        }

        if (!await _usersRepository.IsUserExistsAsync(userId))
        {
            _logger.LogInformation(
                $"UserService: User {userId} try to change password for user that doew not exists");
            throw new InvalidDataException("User does not exist");
        }

        var user = await GetUserByIdAsync(userId);

        if (!BCrypt.Net.BCrypt.Verify(passwordOld, user.Password))
        {
            _logger.LogWarning($"UserService: User with username {user.Username} try to login with invalid password");
            throw new ArgumentException("Password is incorrect");
        }

        await _tokenService.DeactivateTokensForUserAsync(userId);
        await _usersRepository.ChangePasswordAsync(userId, password);

        _logger.LogInformation(
            $"UserService: User {userId} change password succesfull");
    }

    private async Task<string> CreateEmailConfirmTokenAsync(int userId)
    {
        var token = StringUtil.GenerateRandomString(32);

        await _tokensRepository.InsertEmailConfirmationTokenAsync(userId, token);

        return token;
    }

    private ClaimsIdentity GetClaimIdentity(UserModel user)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Sid, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
        };
        var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        _logger.LogInformation($"UserService: User with username {user.Username} login successful");

        return claimsIdentity;
    }

    private (string token, int expires) GenerateToken(ClaimsIdentity identity)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            _jwtSetting.Issuer,
            _jwtSetting.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(_jwtSetting.Lifetime)),
            signingCredentials: new SigningCredentials(_jwtSetting.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        var expires = (int) new DateTimeOffset(now).Add(TimeSpan.FromMinutes(_jwtSetting.Lifetime))
            .ToUnixTimeSeconds();

        return (token, expires);
    }
}