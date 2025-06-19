using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Gateway.Data;
using Transport.Shared.Entities;
using Transport.Shared.Enums;

namespace API.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GatewayDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(GatewayDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent([FromBody] StudentRegistrationRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Email already registered");
        }
        // Require valid OrganizationId
        var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == request.OrganizationId);
        if (org == null)
        {
            return BadRequest("Invalid OrganizationId");
        }
        var student = new Student(request.Email, request.Password)
        {
            Name = request.Name,
            OrganizationId = request.OrganizationId
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        var token = GenerateJwtToken(student);
        return Ok(new { Token = token, User = new { student.Id, student.Email, student.Role } });
    }

    [HttpPost("register/provider")]
    public async Task<IActionResult> RegisterProvider([FromBody] ProviderRegistrationRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Email already registered");
        }
        // Require and validate invite token (dummy logic for now)
        if (string.IsNullOrWhiteSpace(request.InviteToken) || request.InviteToken != "VALID_INVITE_TOKEN")
        {
            return BadRequest("Invalid or missing invite token");
        }
        var provider = new Provider(request.Email, request.Password)
        {
            CompanyName = request.CompanyName,
            LicenseNumber = request.LicenseNumber,
            Name = request.ContactPerson
        };
        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();
        var token = GenerateJwtToken(provider);
        return Ok(new { Token = token, User = new { provider.Id, provider.Email, provider.Role } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !user.VerifyPassword(request.Password))
        {
            return Unauthorized("Invalid email or password");
        }
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var token = GenerateJwtToken(user);
        return Ok(new { Token = token, User = new { user.Id, user.Email, user.Role } });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
        {
            return Unauthorized();
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(new { user.Id, user.Email, user.Role, user.Name, user.LastLoginAt });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyHereMakeItLongEnoughForSecurity"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("UserId", user.Id.ToString())
        };
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "TransportApp",
            audience: _configuration["Jwt:Audience"] ?? "TransportApp",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// DTOs
public class StudentRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
}

public class ProviderRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string InviteToken { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
} 