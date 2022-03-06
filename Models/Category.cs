using System.ComponentModel.DataAnnotations;

namespace trackitback.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public List<Spending>? Spendings { get; set; }
        public List<Income>? Incomes { get; set; }


    }
}
