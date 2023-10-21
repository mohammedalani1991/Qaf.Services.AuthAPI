using Microsoft.AspNetCore.Identity;

namespace ARID.Servicces.AuthAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
