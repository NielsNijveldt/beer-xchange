﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xpirit.BeerXchange.Model;

namespace Xpirit.BeerXchange
{
	public class EditModel : PageModel
    {
        private readonly BeerXchangeContext _context;

        public EditModel(BeerXchangeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Beer Beer { get; set; }

        public List<Beer> SwitchedForBeers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Beer = await _context.Beer.FirstOrDefaultAsync(m => m.Id == id);
            SwitchedForBeers = await _context.Beer.Where(b => b.RemovedBy != "").ToListAsync();

            if (Beer == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Beer.CreatedBy = User.FindFirst("name").Value;
            Beer.AddedDate = DateTime.Now;
            _context.Attach(Beer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeerExists(Beer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BeerExists(int id)
        {
            return _context.Beer.Any(e => e.Id == id);
        }
    }
}
