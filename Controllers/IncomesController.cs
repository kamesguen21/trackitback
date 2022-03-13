#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class IncomesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;

        public IncomesController(DatabaseContext context, IUriService uriService)
        {
            _context = context;
            this.uriService = uriService;
        }

        // GET: api/Incomes/page
        [HttpGet("page")]
        public async Task<ActionResult> GetIncomePage([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Income
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();
            var totalRecords = await _context.Income.CountAsync();
            var pagedReponse = PaginationHelper.CreatePagedReponse<Income>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Income>>> GetIncome()
        {
            return await _context.Income.ToListAsync();
        }


        // GET: api/Incomes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Income>> GetIncome(int id)
        {
            var income = await _context.Income.FindAsync(id);

            if (income == null)
            {
                return NotFound();
            }

            return income;
        }

        // PUT: api/Incomes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncome(int id, Income income)
        {
            if (id != income.Id)
            {
                return BadRequest();
            }

            _context.Entry(income).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncomeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
           await this.UpdateCategory(income.CategoryId);
            return NoContent();
        }

 

        // POST: api/Incomes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Income>> PostIncome(Income income)
        {
            _context.Income.Add(income);
            await _context.SaveChangesAsync();
            await this.UpdateCategory(income.CategoryId);

            return CreatedAtAction("GetIncome", new { id = income.Id }, income);
        }

        // DELETE: api/Incomes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var income = await _context.Income.FindAsync(id);
            if (income == null)
            {
                return NotFound();
            }

            _context.Income.Remove(income);
            await _context.SaveChangesAsync();
            await this.UpdateCategory(income.CategoryId);

            return NoContent();
        }

        private bool IncomeExists(int id)
        {
            return _context.Income.Any(e => e.Id == id);
        }
        private async Task UpdateCategory(int? CategoryId)
        {
            if (CategoryId != null)
            {
                var incomes = await _context.Income.Where(x => x.CategoryId == CategoryId).ToListAsync();
                decimal total = 0;
                incomes.ForEach(x => { total += x.Amount; });
                var category = await _context.Category.FindAsync(CategoryId);
                category.Amount = total;
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
