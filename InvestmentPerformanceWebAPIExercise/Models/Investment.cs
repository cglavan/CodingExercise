namespace InvestmentPerformanceWebAPIExercise.Models
{
    public class Investment
    {
        public Guid InvestmentId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Shares { get; set; }
        public decimal CostBasisPerShare { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
