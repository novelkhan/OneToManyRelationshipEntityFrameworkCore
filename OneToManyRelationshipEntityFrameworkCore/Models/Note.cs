using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneToManyRelationshipEntityFrameworkCore.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string NoteName { get; set; }
        public int NoteValue { get; set; }


        [ForeignKey("PersonId")]
        public int PersonId { get; set; }
    }
}
