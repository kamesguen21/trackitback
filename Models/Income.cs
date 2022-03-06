using System.ComponentModel.DataAnnotations;

namespace trackitback.Models
{
    public class Income
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? InvestmentId { get; set; }
        public Investment? Investment { get; set; }
    }
}
