using System.ComponentModel.DataAnnotations;

namespace trackitback.Models
{
    public record LoginModel
     (
         [Required(ErrorMessage = "User name is required!")] string UserName,
         [Required(ErrorMessage = "Password is required!")] string Password
     );
}
