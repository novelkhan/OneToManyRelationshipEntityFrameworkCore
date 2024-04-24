using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace OneToManyRelationshipEntityFrameworkCore.Components
{
    public class NotesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NotesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            int i = 0;
            var person = await _context.Persons.Include(p => p.ResearvedNotes).FirstOrDefaultAsync(m => m.PersonId == id);
            foreach (var note in person.ResearvedNotes) { i++; }
            ViewBag.AvailableNotes = i;
            return View(person);
        }
    }
}
