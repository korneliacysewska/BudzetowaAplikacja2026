using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    
}
