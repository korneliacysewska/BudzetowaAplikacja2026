using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class StatisticController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IDatabase _redis;

    public StatisticController(ApplicationDbContext db, IConnectionMultiplexer redis)
    {
        _db = db;
        _redis = redis.GetDatabase();
    }

    
    [HttpGet("expenses-by-month")]
    public async Task<IActionResult> ExpensesByMonth()
    {
        
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(new { message = "Brak identyfikacji użytkownika" });

        var cacheKey = $"statistics:expenses-by-month:user:{userId}:v1";

        var cached = await _redis.StringGetAsync(cacheKey);
        if (cached.HasValue)
        {
            var dto = JsonSerializer.Deserialize<ChartDataDto>(cached!, JsonOptions);
            if (dto != null)
                return Ok(dto);
        }

        var grouped = await _db.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId.Value)
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Total = g.Sum(x => x.Amount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        var result = new ChartDataDto
        {
            Labels = grouped.Select(x => $"{x.Year}-{x.Month:D2}").ToList(),
            Data = grouped.Select(x => (double)x.Total).ToList()
        };

        var json = JsonSerializer.Serialize(result, JsonOptions);
        await _redis.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));

        return Ok(result);
    }

    private int? GetUserId()
    {
        
        var claimUserId = User.FindFirst("userId")?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(claimUserId))
            return null;

        return int.TryParse(claimUserId, out var id) ? id : null;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public class ChartDataDto
    {
        public List<string> Labels { get; set; } = new();
        public List<double> Data { get; set; } = new();
    }
}