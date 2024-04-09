namespace OneToManyRelationshipEntityFrameworkCore.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public List<Note>? ResearvedNotes { get; set; }
    }
}
