namespace stajApi.Models.Dtos
{//veritasımak icin
    public class MonthlyReportDto
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
