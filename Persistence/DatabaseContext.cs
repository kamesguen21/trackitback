using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using trackitback.Models;

namespace trackitback.Persistence
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }
        public DbSet<trackitback.Models.Bill> Bill { get; set; }
        public DbSet<trackitback.Models.Category> Category { get; set; }
        public DbSet<trackitback.Models.Goal> Goal { get; set; }
        public DbSet<trackitback.Models.Income> Income { get; set; }
        public DbSet<trackitback.Models.Investment> Investment { get; set; }
        public DbSet<trackitback.Models.Spending> Spending { get; set; }
    }
}