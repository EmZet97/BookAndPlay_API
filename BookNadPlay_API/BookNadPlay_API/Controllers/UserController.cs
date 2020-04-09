using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookAndPlay_API.Models;
using BookNadPlay_API;
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
        private readonly DatabaseContext context;

        public UserController(IConfiguration config, DatabaseContext context)
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
                return BadRequest("siema");
            }

            var _user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());
            if (_user != null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);                
                response.Content = new StringContent("Email already in use!");

                
                return BadRequest("Email already in use!");
            }              

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Ok(user);
        }

        // DELETE USER
        // POST: api/Users/Delete
        [Authorize]
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

        // CHECK IF EMAIL IS IN USE
        // POST: api/Users/Email/Check
        [HttpPost]
        [Route("Email/Check")]
        public async Task<IActionResult> CheckIfUserEmailExists([FromBody] User user)
        {
            var _user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());

            if (_user == null)
            {
                return NotFound();
            }

            return Ok();
        }

        
    }
}