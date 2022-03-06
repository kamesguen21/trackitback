#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackitback.Models;
using trackitback.Persistence;

namespace trackitback.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class InvestmentsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public InvestmentsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Investments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Investment>>> GetInvestment()
        {
            return await _context.Investment.ToListAsync();
        }

        // GET: api/Investments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Investment>> GetInvestment(int id)
        {
            var investment = await _context.Investment.FindAsync(id);

            if (investment == null)
            {
                return NotFound();
            }

            return investment;
        }

        // PUT: api/Investments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvestment(int id, Investment investment)
        {
            if (id != investment.Id)
            {
                return BadRequest();
            }

            _context.Entry(investment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvestmentExists(id))
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

        // POST: api/Investments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Investment>> PostInvestment(Investment investment)
        {
            _context.Investment.Add(investment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvestment", new { id = investment.Id }, investment);
        }

        // DELETE: api/Investments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(int id)
        {
            var investment = await _context.Investment.FindAsync(id);
            if (investment == null)
            {
                return NotFound();
            }

            _context.Investment.Remove(investment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvestmentExists(int id)
        {
            return _context.Investment.Any(e => e.Id == id);
        }
    }
}
