namespace VeteransBannerCRM.Models
{
    public class FinancialTransaction
    {
        public int TransactionID { get; set; }
        public string TransactionDate { get; set; } = "";
        public string Season { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }
}