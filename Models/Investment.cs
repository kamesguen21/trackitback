using System.ComponentModel.DataAnnotations;

namespace trackitback.Models
{
    public class Investment
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Often { get; set; }

    }
}
