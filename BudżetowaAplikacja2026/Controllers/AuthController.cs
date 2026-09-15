using BudżetowaAplikacja2026.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            return Conflict("Email already exists.");

        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            FullName = registerDto.FullName,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Login), new { email = user.Email });
    }

    private static string HashPassword(string password)
    {
       
        password = password?.Trim() ?? "";

        byte[] salt = RandomNumberGenerator.GetBytes(16);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
            return NotFound(new { message = "Konto o podanym email nie istnieje w bazie." });

        var ok = VerifyPassword(loginDto.Password, user.PasswordHash);

        if (!ok)
            return Unauthorized(new
            {
                message = "Nieprawidłowe hasło.",
             
                debug = new
                {
                    storedHash = user.PasswordHash,
                    isSaltHashFormat = (user.PasswordHash ?? "").Contains(':')
                }
            });

        return Ok(new { Token = "your_generated_jwt_token" });
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        password = password?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(storedHash)) return false;

        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;

        try
        {
            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                100_000,
                HashAlgorithmName.SHA256);

            var actualHash = pbkdf2.GetBytes(32);

            return actualHash.SequenceEqual(expectedHash);
        }
        catch
        {
          
            return false;
        }
    }
}


