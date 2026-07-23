namespace UrbanNoodle.Dto
{
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public string MostOrderedByQuantity { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public string MostFrequentInOrders { get; set; } = string.Empty;
        public int OrderCount { get; set; }

        public DashboardSummaryDto()
        {
        }
    }
}
