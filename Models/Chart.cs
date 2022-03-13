using System.ComponentModel.DataAnnotations;

namespace trackitback.Models
{
    public class DashboardData
    {
        public List<Chart>? Charts { get; set; }
        public List<Kpi>? Kpis { get; set; }

    }
    public class Kpi
    {
        public string? Label { get; set; }
        public decimal? Value { get; set; }
        public decimal? percent { get; set; }

    }
    public class Chart
    {
        public string? Title { get; set; }
        public List<Item>? Items { get; set; }
        public string? Period { get; set; }
        public string? Type { get; set; }
        public int? PeriodValue { get; set; }

    }
    public class Item
    {
        public string? Title { get; set; }

        public string? Label { get; set; }
        public decimal? Value { get; set; }
    }
}
