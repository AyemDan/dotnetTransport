using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Interfaces;
using Transport.Domain.Interfaces.MongoDB;
using Transport.Domain.Entities.MongoDB;
using Microsoft.AspNetCore.Authorization;

namespace Transport.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IMongoRepository<ProviderInvite> _inviteRepo;

    public AuthController(IUserRepository userRepository, IConfiguration configuration, IMongoRepository<ProviderInvite> inviteRepo)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _inviteRepo = inviteRepo;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
        {
            return BadRequest(new { message = "Email already in use" });
        }
        
        if (request.Role != UserRole.Student && request.Role != UserRole.Provider && request.Role != UserRole.Driver)
        {
            return BadRequest(new { message = "Role must be either 'Student', 'Provider', or 'Driver'." });
        }

        User? user = request.Role switch
        {
            UserRole.Provider => new Provider(request.Email, request.Password),
            UserRole.Student => new Student(request.Email, request.Password),
            UserRole.Driver => new Driver(request.Email, request.Password),
            _ => null
        };

        if (user == null)
        {
            return BadRequest(new { message = "Invalid role. Choose 'Provider', 'Student', or 'Driver'." });
        }

        await _userRepository.AddUserAsync(user);
        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    [HttpPost("register-provider")]
    public async Task<IActionResult> RegisterProvider([FromQuery] string inviteToken, [FromBody] RegisterRequest request)
    {
        var invites = await _inviteRepo.FindAsync(i => i.Token == inviteToken);
        var invite = invites.FirstOrDefault();
        if (invite == null || invite.Used || invite.Expiry < DateTime.UtcNow)
            return BadRequest(new { message = "Invalid or expired invite token." });
        if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
            return BadRequest(new { message = "Email already in use" });
        if (request.Role != UserRole.Provider)
            return BadRequest(new { message = "Role must be 'Provider'." });
        var user = new Provider(request.Email, request.Password);
        await _userRepository.AddUserAsync(user);
        invite.Used = true;
        await _inviteRepo.UpdateAsync(invite.Id.ToString(), invite);
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

    [HttpPost("register-org-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterOrgAdmin([FromBody] RegisterOrgAdminRequest request)
    {
        // Only platform Admins can create org admins
        var orgAdmin = new OrganizationAdmin(request.Email, request.Password, request.OrganizationId);
        await _userRepository.AddUserAsync(orgAdmin);
        // Optionally, send invite email or return token
        return Ok(new { message = "Organization admin created." });
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
    [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be either 'Student', 'Provider', or 'Driver'.")]
    public UserRole Role { get; set; } // "provider" or "student" or "driver"
}

public class LoginRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

public class RegisterOrgAdminRequest
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public Guid OrganizationId { get; set; }
}
