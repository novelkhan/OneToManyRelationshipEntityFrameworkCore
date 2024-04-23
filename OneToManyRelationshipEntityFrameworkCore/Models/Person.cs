using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneToManyRelationshipEntityFrameworkCore.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        public string Name { get; set; }
        public List<Note>? ResearvedNotes { get; set; }
    }
}