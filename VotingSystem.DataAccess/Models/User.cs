using System.ComponentModel.DataAnnotations;
/*
Egy felhasználót reprezentáló osztály.
Id - Azonosító
Email - E-mail cím
PasswordHash - Jelszó hash
Votes - Azok a szavazatok, amiket a felhasználó leadott
*/

namespace VotingSystem.DataAccess {

    public class User 
    {
        [Key]
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    }
}