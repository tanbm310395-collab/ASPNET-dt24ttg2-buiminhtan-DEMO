namespace money_management.Models
{
    public class ReportItem
    {
        public string CategoryName { get; set; } = "100";
        public decimal TotalAmount { get; set; } = 0;
        public string Type { get; set; } = "Income"; // income hoặc expense
    }
}
