using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace OneToManyRelationshipEntityFrameworkCore.Components
{
    public class PersonViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public PersonViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == id);
            return View(person);
        }
    }
}
