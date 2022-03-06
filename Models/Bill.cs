
using System.ComponentModel.DataAnnotations;
namespace trackitback.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Type { get; set; }
         public string? Often { get; set; }
         [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public List<Spending>? Spendings { get; set; }
    }
}
