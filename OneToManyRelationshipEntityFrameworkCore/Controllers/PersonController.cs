using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneToManyRelationshipEntityFrameworkCore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneToManyRelationshipEntityFrameworkCore.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Person
        public async Task<IActionResult> Index()
        {
            return View(await _context.Persons.ToListAsync());
        }

        // GET: Person/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            //var person = await _context.Person.FirstOrDefaultAsync(m => m.Id == id);            
            var person = await _context.Persons.Include(p => p.ResearvedNotes).AsNoTracking().FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }


        public async Task<IActionResult> Detail(int? PersonId)
        {
            if (PersonId == null || _context.Persons == null)
            {
                return NotFound();
            }

            //var person = await _context.Person.FirstOrDefaultAsync(m => m.Id == id);            
            var person = await _context.Persons.Include(p => p.ResearvedNotes).AsNoTracking().FirstOrDefaultAsync(m => m.PersonId == PersonId);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: Person/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Person/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,Name")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: Person/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,Name")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: Person/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(p => p.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Persons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Person'  is null.");
            }

            var person = await _context.Persons.Include(r => r.ResearvedNotes).AsNoTracking().FirstOrDefaultAsync(p => p.PersonId == id);

            if (person != null)
            {
                if (person.ResearvedNotes != null)
                {
                    foreach (var bankNote in person.ResearvedNotes)
                    {
                        var noteToBeRemoved = await _context.Notes.FindAsync(bankNote.NoteId);
                        if (noteToBeRemoved != null)
                        {
                            _context.Notes.Remove(noteToBeRemoved);
                        }
                    }

                    person.ResearvedNotes = null;
                }

                _context.Persons.Remove(person);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }







        //------------------------------------Custom Codes Start-----------------------------------------



        // GET: Note/Add
        public IActionResult Add(int Id)
        {
            return View();
        }




        // POST: Note/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int Id, [Bind("NoteId,NoteName,NoteValue,PersonId")] Note note)
        {
            int PersonId = Id;
            if (ModelState.IsValid)
            {
                var person = await _context.Persons.FindAsync(PersonId);
                //person.notes.Add(note);
                if (person != null)
                {
                    if (person.ResearvedNotes == null)
                    {
                        person.ResearvedNotes = new List<Note>();
                        person.ResearvedNotes.Add(note);
                    }
                    else
                    {
                        person.ResearvedNotes.Add(note);
                    }


                    if (person.ResearvedNotes != null)
                    {
                        //_context.Entry(person).State = EntityState.Added; // added row

                        try
                        {
                            _context.Entry(person).State = EntityState.Modified;
                            _context.Persons.Update(person);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!PersonExists(person.PersonId))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }


                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Details", "Person", new { PersonId });
                }
            }
            return View(note);
        }















        public async Task<IActionResult> DeleteNote(int PersonId, int NoteId)
        {
            if (NoteId == null || _context.Notes == null)
            {
                return NotFound();
            }

            var note = await _context.Notes.Where(i => i.PersonId == PersonId).FirstOrDefaultAsync(m => m.NoteId == NoteId);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Note/Delete/5
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNoteConfirmed(int PersonId, int NoteId)
        {
            if (_context.Notes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Notes'  is null.");
            }

            var noteToBeRemoved = await _context.Notes.Where(i => i.PersonId == PersonId).FirstOrDefaultAsync(m => m.NoteId == NoteId);
            if (noteToBeRemoved != null)
            {
                _context.Notes.Remove(noteToBeRemoved);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Person", new { PersonId });
        }














        // GET: Note/Edit/5
        public async Task<IActionResult> EditNote(int PersonId, int NoteId)
        {
            if (NoteId == null || _context.Notes == null)
            {
                return NotFound();
            }

            //var note = await _context.Note.FindAsync(NoteId);
            var noteToBeEdited = await _context.Notes.Where(i => i.PersonId == PersonId).FirstOrDefaultAsync(m => m.NoteId == NoteId);
            if (noteToBeEdited == null)
            {
                return NotFound();
            }
            return View(noteToBeEdited);
        }

        // POST: Note/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNote(int PersonId, int NoteId, [Bind("NoteId,NoteName,NoteValue,PersonId")] Note note)
        {
            if (NoteId != note.NoteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.NoteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", "Person", new { PersonId });
            }
            return View(note);
        }













        // GET: Note/Details/5
        public async Task<IActionResult> NoteDetails(int PersonId, int NoteId)
        {
            if (NoteId == null || _context.Notes == null)
            {
                return NotFound();
            }

            //var note = await _context.Note.FirstOrDefaultAsync(m => m.Id == id);
            var note = await _context.Notes.Where(p => p.PersonId == PersonId).FirstOrDefaultAsync(m => m.NoteId == NoteId);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }








        public async Task<IActionResult> AllNotes()
        {
            return _context.Notes != null ?
                        View(await _context.Notes.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Notes'  is null.");
        }



        //public async Task<IActionResult> AllNotes()
        //{
        //    dynamic dm = new ExpandoObject();
        //    dm.note = _context.Notes.ToListAsync();
        //    dm.person = _context.Person.ToListAsync();
        //    return _context.Notes != null ?
        //                View(dm) :
        //                Problem("Entity set 'ApplicationDbContext.Note'  is null.");
        //}







        //------------------------------------Custom Codes End-----------------------------------------








        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.PersonId == id);
        }

        private bool NoteExists(int NoteId)
        {
            return (_context.Notes?.Any(e => e.NoteId == NoteId)).GetValueOrDefault();
        }
    }
}
