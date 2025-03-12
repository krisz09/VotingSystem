
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
/*
Egy szavazatot reprezentáló osztály.
Id - Azonosító
UserId - A felhasználó azonosítója, aki a szavazatot leadta
PollOptionId - A szavazás lehetőségének azonosítója, amire a szavazat érkezett
VotedAt - A szavazat leadásának időpontja
User - A felhasználó, aki a szavazatot leadta
PollOption - A szavazás lehetősége, amire a szavazat érkezett
*/

namespace VotingSystem.DataAccess {

    public class Vote
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("PollOption")]
        public int PollOptionId { get; set; }
        public DateTime VotedAt { get; set; }
        public virtual User user { get; set; } = null!;
        public virtual PollOption PollOption { get; set; } = null!;
    }
}