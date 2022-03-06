using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace trackitback.Persistence
{
    public class User : IdentityUser
    {
        [Required] public string? FullName { get; set; }
    }
}