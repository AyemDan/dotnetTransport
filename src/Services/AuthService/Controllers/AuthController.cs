using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Transport.Shared.Entities;
using Transport.Shared.Enums;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMongoCollection<AppUser> _users;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
        IMongoDatabase database,
        ILogger<AuthController> logger,
        IConfiguration configuration
    )
    {
        _users = database.GetCollection<AppUser>("users");
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _users
                .Find(u => u.Email == registrationDto.Email)
                .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists");
            }

            // Create new AppUser
            var user = new AppUser(registrationDto.Email, registrationDto.Password)
            {
                Role = Enum.Parse<UserRole>(registrationDto.Role),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            await _users.InsertOneAsync(user);

            return Ok(new { Message = "User registered successfully", UserId = user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await _users.Find(u => u.Email == loginDto.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password");
            }

            if (!user.IsActive)
            {
                return Unauthorized("Account is deactivated");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Update last login
            var update = Builders<AppUser>.Update.Set(u => u.LastLoginAt, DateTime.UtcNow);

            await _users.UpdateOneAsync(u => u.Id == user.Id, update);

            return Ok(
                new
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    ExpiresIn = 3600, // 1 hour
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken([FromBody] TokenValidationDto tokenDto)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "YourSuperSecretKeyHereMakeItLongEnoughForSecurity"
            );

            tokenHandler.ValidateToken(
                tokenDto.Token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"] ?? "TransportApp",
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"] ?? "TransportApp",
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validatedToken
            );

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;

            var user = await _users.Find(u => u.Id == Guid.Parse(userId)).FirstOrDefaultAsync();
            if (user == null || !user.IsActive)
            {
                return Unauthorized("Invalid token");
            }

            return Ok(
                new
                {
                    IsValid = true,
                    UserId = user.Id,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return Ok(new { IsValid = false });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshDto)
    {
        try
        {
            var user = await _users.Find(u => u.Id == Guid.Parse(refreshDto.UserId)).FirstOrDefaultAsync();
            if (user == null || !user.IsActive)
            {
                return Unauthorized("Invalid user");
            }

            // Generate new JWT token
            var token = GenerateJwtToken(user);

            return Ok(
                new
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    ExpiresIn = 3600, // 1 hour
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutDto logoutDto)
    {
        try
        {
            var user = await _users.Find(u => u.Id == Guid.Parse(logoutDto.UserId)).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Update last logout
            var update = Builders<AppUser>.Update.Set(u => u.LastLogoutAt, DateTime.UtcNow);
            await _users.UpdateOneAsync(u => u.Id == user.Id, update);

            return Ok(new { Message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        try
        {
            var user = await _users.Find(u => u.Id == Guid.Parse(userId)).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(
                new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("users/{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        string userId,
        [FromBody] UserStatusUpdateDto statusDto
    )
    {
        try
        {
            var user = await _users.Find(u => u.Id == Guid.Parse(userId)).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User not found");
            }

            var update = Builders<AppUser>.Update
                .Set(u => u.IsActive, statusDto.IsActive)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            await _users.UpdateOneAsync(u => u.Id == user.Id, update);

            return Ok(
                new
                {
                    Message = "User status updated successfully",
                    UserId = user.Id,
                    IsActive = statusDto.IsActive,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status");
            return StatusCode(500, "Internal server error");
        }
    }

    private string GenerateJwtToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "YourSuperSecretKeyHereMakeItLongEnoughForSecurity"
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim("role", user.Role.ToString()),
                }
            ),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"] ?? "TransportApp",
            Audience = _configuration["Jwt:Audience"] ?? "TransportApp",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

// DTOs for Auth Service
public class UserRegistrationDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TokenValidationDto
{
    public string Token { get; set; } = string.Empty;
}

public class RefreshTokenDto
{
    public string UserId { get; set; } = string.Empty;
}

public class LogoutDto
{
    public string UserId { get; set; } = string.Empty;
}

public class UserStatusUpdateDto
{
    public bool IsActive { get; set; }
}
