using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
/*
Egy felhasználót reprezentáló osztály.
Id - Azonosító
Email - E-mail cím
PasswordHash - Jelszó hash
Votes - Azok a szavazatok, amiket a felhasználó leadott
*/

namespace VotingSystem.DataAccess {

    public class User : IdentityUser
    {
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}