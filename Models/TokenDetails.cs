using System;
namespace trackitback.Models
{
    public record TokenDetails
     (
         string Token,
         DateTime Expiration
     );
}
