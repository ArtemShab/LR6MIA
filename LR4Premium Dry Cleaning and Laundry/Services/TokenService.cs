using LR4Premium_Dry_Cleaning_and_Laundry.Configuration;
using DryCleaningApi.DTOs;
using DryCleaningApi.Models;
using DryCleaningApi.Repositories.Interfaces;
using DryCleaningApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace DryCleaningApi.Services;
public class TokenService : ITokenService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public TokenService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Користувач із таким email вже існує.");

        var role = (dto.Role == "Admin" || dto.Role == "User") ? dto.Role : "User";

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = role,
            RefreshToken = GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _userRepository.CreateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = user.RefreshToken!,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || user.PasswordHash != HashPassword(dto.Password))
            throw new UnauthorizedAccessException("Невірний email або пароль.");

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _userRepository.UpdateAsync(user.Id!, user);

        return new AuthResponseDto
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = user.RefreshToken,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Недійсний або прострочений refresh token.");

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _userRepository.UpdateAsync(user.Id!, user);

        return new AuthResponseDto
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = user.RefreshToken,
            Email = user.Email,
            Role = user.Role
        };
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}