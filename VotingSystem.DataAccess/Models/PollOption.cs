using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
/*
Egy szavazás lehetőséget reprezentáló osztály.
Id - Azonosító
OptionText - A lehetőség szövege
PollId - Annak a szavazásnak az azonosítója, amihez tartozik a lehetőség
Poll - Az a szavazás, amihez tartozik a lehetőség
Votes - Azok a szavazatok, amik erre a lehetőségre érkeztek
*/

namespace VotingSystem.DataAccess {

    public class PollOption {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OptionText { get; set; } = null!;
        [ForeignKey("Poll")]
        public int PollId { get; set; }

        public virtual Poll Poll { get; set; } = null!;
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}