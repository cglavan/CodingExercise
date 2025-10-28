using System.ComponentModel.DataAnnotations;

namespace InvestmentPerformanceWebAPIExercise.Data
{
    public class InvestmentDto
    {
        public Guid InvestmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

    }
}
