using InvestmentPerformanceWebAPIExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPerformanceWebAPIExercise.Data
{
    public class InvestmentDbContext : DbContext
    {
        public DbSet<Investment> Investments { get; set; }

        public InvestmentDbContext(DbContextOptions<InvestmentDbContext> options)
            : base(options) { }
    }
}
