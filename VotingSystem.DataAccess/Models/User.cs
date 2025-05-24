using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VotingSystem.DataAccess
{
    /*
    Egy felhasználót reprezentáló osztály.
    Id - Azonosító
    Email - E-mail cím
    PasswordHash - Jelszó hash
    Votes - Azok a szavazatok, amiket a felhasználó leadott
    RefreshToken - Új JWT token igénylésére használható token
    */
    public class User : IdentityUser
    {
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

        public Guid? RefreshToken { get; set; }

    }
}