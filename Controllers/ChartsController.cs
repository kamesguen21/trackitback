#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackitback.Filter;
using trackitback.Helpers;
using trackitback.Models;
using trackitback.Persistence;
using trackitback.Services;
using trackitback.Wrappers;

namespace trackitback.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public ChartsController(DatabaseContext context )
        {
            _context = context;
        }
        [HttpGet("import")]
        public async Task<IActionResult> UploadData()
        {
        string jsonString = System.IO.File.ReadAllText("./data/data.json");

            Data Importdata = JsonSerializer.Deserialize<Data>(jsonString);
            if(Importdata != null && Importdata.expenses.Count>0 && Importdata.categories.Count>0 && Importdata.incomes.Count>0 )
            {
                List<Category> categories = await _context.Category.ToListAsync();
                categories.ForEach(c =>
                {
                    _context.Category.Remove(c);
                  
                });

                List<Income> Incomes = await _context.Income.ToListAsync();
                Incomes.ForEach(c =>
                {
                    _context.Income.Remove(c);

                });
                List<Spending> Spendings = await _context.Spending.ToListAsync();
                Spendings.ForEach(c =>
                {
                    _context.Spending.Remove(c);

                });
                await _context.SaveChangesAsync();

                Importdata.categories.ForEach(c => {
                    _context.Category.Add(c);
                });
                await _context.SaveChangesAsync();
                categories = await _context.Category.ToListAsync();
                List<Category> IncomesCategories = categories.FindAll(e => e.Type == "INCOME");
                List<Category> ExpenseCategories = categories.FindAll(e => e.Type == "EXPENSE");

                Importdata.incomes.ForEach(income =>
                {
                    var random = new Random();
                    int index = random.Next(IncomesCategories.Count-1);
                    income.CategoryId = IncomesCategories[index].Id;
                    _context.Income.Add(income);
                });
                Importdata.expenses.ForEach(expense =>
                {
                    var random = new Random();
                    int index = random.Next(ExpenseCategories.Count-1);
                    expense.CategoryId = ExpenseCategories[index].Id;
                    _context.Spending.Add(expense);
                });
                await _context.SaveChangesAsync();
                categories.ForEach(c =>
                {
                   this.UpdateCategory(c.Id);
                });
           
            }
            return NoContent();

        }
        [HttpGet]
        public async Task<ActionResult<DashboardData>> GetCharts()
        {
            DashboardData dashboardData= new DashboardData();   
            List<Chart> Charts = new List<Chart>();
            decimal TtIncomes = 0;
            decimal TtSpendings = 0;
            int CountIncomes = 0;
            int CountSpendings = 0;
            List<Income> Incomes = await _context.Income.ToListAsync();
            List<Spending> Spendings = await _context.Spending.ToListAsync();
            Incomes.ForEach(Income => { TtIncomes += Income.Amount; CountIncomes++; });
            Spendings.ForEach(Spending => { TtSpendings += Spending.Amount; CountSpendings++; });

            Chart YearIncomeChart = new Chart();
            YearIncomeChart.Period = "YEARS";
            YearIncomeChart.Title = "Incomes per year";
            YearIncomeChart.Type = "INCOME";

            YearIncomeChart.Items = new List<Item>();
            Chart YearSpendingChart = new Chart();
            YearSpendingChart.Period = "YEARS";
            YearSpendingChart.Title = "Spending per year";
            YearSpendingChart.Items = new List<Item>();
            YearIncomeChart.Type = "INCOME";
            YearSpendingChart.Type = "SPENDING";

            var dateOnly = DateOnly.FromDateTime(DateTime.Now);
            int year = dateOnly.Year;
            for (int i = 10; i >0 ; i--)
            {
                decimal incomeYearTotal = 0;
                Incomes.ForEach(x =>
                {
                    if (x.Date.Year == year)
                    {
                        incomeYearTotal += x.Amount;
                    }
                });
                Item incomeYearItem = new Item();
                incomeYearItem.Title =""+year;
                incomeYearItem.Label = "" + year;
                incomeYearItem.Value = incomeYearTotal;
                YearIncomeChart.Items.Add(incomeYearItem);

                //SPENDING
                decimal spendingYearTotal = 0;
                Spendings.ForEach(x =>
                {
                    if (x.Date.Year == year)
                    {
                        spendingYearTotal += x.Amount;
                    }
                });
                Item spendingYearItem = new Item();
                spendingYearItem.Title = "" + year;
                spendingYearItem.Label = "" + year;
                spendingYearItem.Value = spendingYearTotal;

                YearSpendingChart.Items.Add(spendingYearItem);
                year--;
            }
            Charts.Add(YearSpendingChart);
            Charts.Add(YearIncomeChart);
            //MONTHS
         
      
            var dateOnly2 = DateOnly.FromDateTime(DateTime.Now);
            int year2 = dateOnly2.Year;
            for (int y = 10; y > 0; y--)
            {
                Chart MonthIncomeChart = new Chart();
                MonthIncomeChart.Period = "MONTHS";
                MonthIncomeChart.Title = "Incomes per month";
                MonthIncomeChart.Items = new List<Item>();
                MonthIncomeChart.PeriodValue = year2;
                MonthIncomeChart.Type = "INCOME";

                Chart MonthSpendingChart = new Chart();
                MonthSpendingChart.Period = "MONTHS";
                MonthSpendingChart.Title = "Spending per month";
                MonthSpendingChart.Type = "SPENDING";
                MonthSpendingChart.PeriodValue = year2;

                MonthSpendingChart.Items = new List<Item>();
                int month = 12;
                for (int i = month; i > 0; i--)
                {

                    decimal incomeMonthTotal = 0;
                    Incomes.ForEach(x =>
                    {
                        if (x.Date.Month == month && x.Date.Year == year2)
                        {
                            incomeMonthTotal += x.Amount;
                        }
                    });
                    Item incomeMonthItem = new Item();
                    incomeMonthItem.Title = "" + month;
                    incomeMonthItem.Label = "" + month;
                    incomeMonthItem.Value = incomeMonthTotal;
                    MonthIncomeChart.Items.Add(incomeMonthItem);
                    //SPENDING
                    decimal spendingMonthTotal = 0;
                    Spendings.ForEach(x =>
                    {
                        if (x.Date.Month == month && x.Date.Year == year2)
                        {
                            spendingMonthTotal += x.Amount;
                        }
                    });
                    Item spendingMonthItem = new Item();
                    spendingMonthItem.Title = "" + month;
                    spendingMonthItem.Label = "" + month;
                    spendingMonthItem.Value = spendingMonthTotal;
                    MonthSpendingChart.Items.Add(spendingMonthItem);
             

                    dateOnly.AddMonths(-1);
                    month--;
                }
                year2--;
                Charts.Add(MonthIncomeChart);
                Charts.Add(MonthSpendingChart);

            }
            dashboardData.Charts= Charts;
            List<Kpi>kpis = new List<Kpi>();
            Kpi balance=new Kpi();
            balance.Value = TtIncomes - TtSpendings;
            balance.Label = "balance";
            balance.percent = (TtIncomes - TtSpendings)/ TtSpendings*100;
            kpis.Add(balance);
            Kpi inc = new Kpi();
            inc.Value = TtIncomes;
            inc.Label = "Income";
            inc.percent = (TtIncomes - TtSpendings) / TtSpendings * 100;
            kpis.Add(inc);
            Kpi spen = new Kpi();
            spen.Value = TtSpendings;
            spen.Label = "Expenses";
            spen.percent = (TtSpendings - TtIncomes) / TtIncomes * 100;
            kpis.Add(spen);
            Kpi count = new Kpi();
            count.Value = CountIncomes+CountSpendings;
            count.Label = "Entries";
            count.percent = (CountIncomes - CountSpendings) / CountSpendings * 100;
            kpis.Add(count);
            dashboardData.Kpis=kpis;
            return dashboardData;
        }

        private void UpdateCategory(int? CategoryId)
        {
            if (CategoryId != null)
            {
                var category = _context.Category.Find(CategoryId);
                if(category != null && category.Type== "EXPENSE")
                {
                    var expenses = _context.Spending.Where(x => x.CategoryId == CategoryId).ToList();
                    decimal total = 0;
                    expenses.ForEach(x => { total += x.Amount; });
                    category.Amount = total;
                    _context.Entry(category).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    var incomes = _context.Income.Where(x => x.CategoryId == CategoryId).ToList();
                    decimal total = 0;
                    incomes.ForEach(x => { total += x.Amount; });
                    category.Amount = total;
                    _context.Entry(category).State = EntityState.Modified;
                    _context.SaveChanges();
                }
          
            }
        }
    }
}