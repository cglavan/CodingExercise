namespace InvestmentPerformanceWebAPIExercise.Data
{
    public class InvestmentDetailDto
    {
        public int Shares { get; set; }
        public decimal CostBasisPerShare { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CurrentValue => Shares * CurrentPrice;
        public string Term { get; set; } = string.Empty;
        public decimal TotalGainLoss { get; set; }
    }
}
