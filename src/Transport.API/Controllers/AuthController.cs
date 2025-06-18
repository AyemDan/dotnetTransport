using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Interfaces;

namespace Transport.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
    {
        return BadRequest(new { message = "Email already in use" });
    }
    
    if (request.Role != UserRole.Student && request.Role != UserRole.Provider)
    {
        return BadRequest(new { message = "Role must be either 'Student' or 'Provider'." });
    }

    User? user = request.Role switch
    {
        UserRole.Provider => new Provider(request.Email, request.Password),
        UserRole.Student => new Student(request.Email, request.Password),
        _ => null
    };

    if (user == null)
    {
        return BadRequest(new { message = "Invalid role. Choose 'Provider' or 'Student'." });
    }

    await _userRepository.AddUserAsync(user);
    var token = GenerateJwtToken(user);

    return Ok(new { token });
}


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null || !user.VerifyPassword(request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.GetType().Name)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class RegisterRequest
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be either 'Student' or 'Provider'.")]
    public UserRole Role { get; set; } // "provider" or "student"
}

public class LoginRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
