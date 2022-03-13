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

namespace trackitback.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SpendingsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        public SpendingsController(DatabaseContext context, IUriService uriService)
        {
            _context = context;
            this.uriService = uriService;

        }

        // GET: api/Spendings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Spending>>> GetSpending()
        {
            return await _context.Spending.ToListAsync();
        }
        // GET: api/Spendings/page
        [HttpGet("page")]
        public async Task<ActionResult> GetSpendingsPage([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Spending
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();
            var totalRecords = await _context.Spending.CountAsync();
            var pagedReponse = PaginationHelper.CreatePagedReponse<Spending>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }
        // GET: api/Spendings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Spending>> GetSpending(int id)
        {
            var spending = await _context.Spending.FindAsync(id);

            if (spending == null)
            {
                return NotFound();
            }

            return spending;
        }

        // PUT: api/Spendings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpending(int id, Spending spending)
        {
            if (id != spending.Id)
            {
                return BadRequest();
            }

            _context.Entry(spending).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpendingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Spendings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Spending>> PostSpending(Spending spending)
        {
            _context.Spending.Add(spending);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpending", new { id = spending.Id }, spending);
        }

        // DELETE: api/Spendings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpending(int id)
        {
            var spending = await _context.Spending.FindAsync(id);
            if (spending == null)
            {
                return NotFound();
            }

            _context.Spending.Remove(spending);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpendingExists(int id)
        {
            return _context.Spending.Any(e => e.Id == id);
        }
    }
}
