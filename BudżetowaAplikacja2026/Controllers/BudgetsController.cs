using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudżetowaAplikacja2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BudgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserBudget(int userId)
        {
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId);
            return Ok(budget ?? new Budget());
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] Budget budget)
        {
            budget.CreatedAt = DateTime.UtcNow;
            await _context.Budgets.AddAsync(budget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserBudget), new { userId = budget.UserId }, budget);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateBudget(int userId, [FromBody] Budget budget)
        {
            if (userId != budget.UserId)
                return BadRequest();

            _context.Entry(budget).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
