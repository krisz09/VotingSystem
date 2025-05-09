using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
Egy reprezentáló osztály.
Id - Azonosító
Question - A kérdés
StartDate - A szavazás kezdeti időpontja
EndDate - A szavazás befejezési időpontja
CreatedByUserId - A szavazást létrehozó felhasználó azonosítója
CreatedByUser - A szavazást létrehozó felhasználó
PollOptions - A szavazás lehetőségei
*/
namespace VotingSystem.DataAccess {
    public class Poll {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Question { get; set; } = null!;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [ForeignKey("User")]
        public string CreatedByUserId { get; set; }
        public virtual User CreatedByUser { get; set; } = null!;
         
        public int Minvotes { get; set; } = 1;
        public int Maxvotes { get; set; }

        public virtual ICollection<PollOption> PollOptions { get; set; } = new List<PollOption>();
        
    }
}
