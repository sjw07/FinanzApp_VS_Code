using System;

namespace FinanzApp.WPF.Data
{
    public class FinanceEntry
    {
        public int EntryId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public EntryType Type { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
    }

    public enum EntryType
    {
        Income,
        Expense
    }
}
