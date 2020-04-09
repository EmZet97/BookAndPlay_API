using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookAndPlay_API.Models;
using BookNadPlay_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookNadPlay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DataContext context;

        public UserController(IConfiguration config, DataContext context)
        {
            this.configuration = config;
            this.context = context;
        }
        

        // AUTHORIZE USER
        // POST: api/User/Auth
        [HttpPost]
        [Route("Auth")]
        public async Task<IActionResult> AuthorizeUser([FromBody] User user)
        {
            if (user != null)
            {
                var _user = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

                if (_user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Role", "User"),
                    new Claim("Id", _user.UserId.ToString()),
                    new Claim("Name", _user.Name),
                    new Claim("Surname", _user.Surname),
                    new Claim("Email", _user.Email)
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    //Return token
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest("Incorrect data");
            }
        }

        // ADD USER
        // POST: api/Users/Add
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Ok(user);
        }

        // ADD USER
        // POST: api/Users/Delete
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteUser()
        {
            //Get user id from token
            var idClaim = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            int id = int.Parse(idClaim.Value);

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if(user!= null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();                
                
                return Ok("User succesfully deleted");
            }

            return BadRequest();
            
        }

        // GET USER FROM TOKEN
        // GET: api/User/Get
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public async Task<User> GetUser()
        {
            //Get user id from token
            var idClaim = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            int id = int.Parse(idClaim.Value);

            return await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<User> GetAllUser()
        {
            //Get user id from token
            //var idClaim = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            //int id = int.Parse(idClaim.Value);

            return context.Users;
        }

        //
        // GET: api/Users/Check
        [HttpGet]
        [Route("Check")]
        public async Task<IActionResult> CheckIfUserExists([FromBody] string email)
        {
            var _user = await context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (_user == null)
            {
                return NotFound();
            }

            return Ok();
        }

        
    }
}