using Microsoft.AspNetCore.Identity;

namespace BestStoreMVC.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? PersonName { get; set; }
    }
}
