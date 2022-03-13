
using System.ComponentModel.DataAnnotations;
namespace trackitback.Models
{
    public class Data
    {
        public List<Category>? categories { get; set; }
        public List<Income>? incomes { get; set; }
        public List<Spending>? expenses { get; set; }
    }
}
