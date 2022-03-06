using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using trackitback.Models;
using trackitback.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace trackitback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public AuthController(UserManager<User> userManager) => this.userManager = userManager;

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var claims = new List<Claim>
        {
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
                byte[] key = Encoding.ASCII.GetBytes("SecretKeywqewqeqqqqqqqqqqqweeeeeeeeeeeeeeeeeeeqweqe");
                var signingKey = new SymmetricSecurityKey(key);
                var token = new JwtSecurityToken(
                    issuer: "http://localhost:7270",
                    audience: "http://localhost:7270",
                    expires: DateTime.Now.AddDays(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
                return new OkObjectResult(new TokenDetails(
                    Token: new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration: token.ValidTo
                ));
            }
            return new BadRequestObjectResult($"User with name {model.UserName} couldn't be found");
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Password != model.ConfirmPassword)
            {
                return new BadRequestObjectResult("Passwords do no match");
            }

            if (await userManager.FindByNameAsync(model.UserName) is not null)
            {
                return new BadRequestObjectResult("Username is taken");
            }
            var user = new User
            {
                UserName = model.UserName,
                FullName = model.FullName,
                Email = model.Email
            };
            var userCreated = await userManager.CreateAsync(user, model.Password);
            if (!userCreated.Succeeded)
            {
                return new BadRequestObjectResult(string.Join(", ", userCreated.Errors.Select(error => $"{error.Code} {error.Description}")));
            }
            return new OkObjectResult(user);
        }  
        [HttpGet]
        [Authorize]
        [Route("account")]
        public async Task<IActionResult> Account()
        {
            string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByNameAsync(id);
            if(user == null)
            {
                return new NotFoundObjectResult("Username not found"+ id);

            }
            return new OkObjectResult(user);
        }  
        [HttpPut]
        [Authorize]
        [Route("account/{id}")]
        public async Task<IActionResult> UpdateAccount(string id, User account)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user==null || user.Id!=account.Id)
            {
                return BadRequest(ModelState);
            }
            user.FullName=account.FullName; 
            user.Email=account.Email; 
            user.UserName=account.UserName; 
            var saved = await userManager.UpdateAsync(user);
            if (saved == null)
            {
                return new NotFoundObjectResult("Username not saved "+ id);

            }
            return new OkObjectResult(saved);
        }
    }
}
