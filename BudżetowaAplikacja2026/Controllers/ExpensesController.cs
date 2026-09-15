using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudżetowaAplikacja2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] Expense expense)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            expense.CreatedAt = DateTime.UtcNow;
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserExpenses), new { id = expense.Id }, expense);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserExpenses(int userId)
        {
            var expenses = await _context.Expenses.Where(e => e.UserId == userId).ToListAsync();
            return Ok(expenses);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense expense)
        {
            if (id != expense.Id)
                return BadRequest();

            _context.Entry(expense).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
