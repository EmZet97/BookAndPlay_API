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
using BookNadPlay_API.Models;
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
        public async Task<IActionResult> AuthorizeUser(AuthModel user)
        {
            if (user != null)
            {
                var _user = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
                
                if (_user != null)
                {
                    UserRoles role = (UserRoles)_user.RoleId;

                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Role", role.ToString()),
                    new Claim("Id", _user.UserId.ToString()),
                    new Claim("Name", _user.Name),
                    new Claim("Surname", _user.Surname),
                    new Claim("Email", _user.Email)
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: signIn);

                    string gen_token = new JwtSecurityTokenHandler().WriteToken(token);
                    
                    var token_model = new TokenModel() { Token = gen_token };

                    return Ok(token_model);
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
        // POST: api/User/Add
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            
            if (!ModelState.IsValid)
            {
                return Unauthorized();
            }

            var _user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());
            if (_user != null)
            {                
                return BadRequest("Email already in use!");
            }

            user.RoleId = (int)UserRoles.Normal;
            user.RoleName = UserRoles.Normal.ToString();

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Ok(user);
        }

        // DELETE LOGGED USER
        // DELETE: api/User/SelfDelete
        [Authorize]
        [HttpDelete]
        [Route("SelfDelete")]
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

            return Unauthorized();

        }

        // DELETE USER
        // DELETE: api/User/Delete
        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteOtherUser(int id)
        {
            //Get user id from token
            var idClaim = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            int _id = int.Parse(idClaim.Value);


            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == _id);
            if (user != null)
            {
                if((UserRoles)user.RoleId == UserRoles.Admin)
                {
                    var _user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
                    if(_user != null && (UserRoles)_user.RoleId != UserRoles.Admin)
                    {
                        context.Users.Remove(_user);
                        await context.SaveChangesAsync();

                        return Ok("User succesfully deleted");
                    }
                    return BadRequest("Incorrect user Id or user is Admin");
                }
                return BadRequest("No permission to user deletion");
            }

            return Unauthorized();

        }

        // EDIT LOGGED USER
        // POST: api/User/SelfEdit
        [Authorize]
        [HttpPost]
        [Route("SelfEdit")]
        public async Task<IActionResult> EditUser([FromBody] User_UpdateModel new_data)
        {
            //Get user id from token
            var idClaim = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            int id = int.Parse(idClaim.Value);

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user != null)
            {
                user.Email = new_data.Email ?? user.Email;
                user.Name = new_data.Name ?? user.Name;
                user.Surname = new_data.Surname ?? user.Surname;
                user.PhoneNumber = new_data.PhoneNumber ?? user.PhoneNumber;
                user.Password = new_data.Password ?? user.Password;

                await context.SaveChangesAsync();

                return Ok("User succesfully edited");
            }

            return Unauthorized();

        }

        // Update USER
        // POST: api/User/Edit
        [Authorize]
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditOtherUser(int id, [FromBody] User_UpdateModel new_data)
        {
            //Get user id from token
            var idClaim = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            int _id = int.Parse(idClaim.Value);

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == _id);
            if (user != null)
            {
                if ((UserRoles)user.RoleId == UserRoles.Admin)
                {
                    var _user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
                    if (_user != null && (UserRoles)_user.RoleId != UserRoles.Admin)
                    {
                        _user.Email = new_data.Email ?? _user.Email;
                        _user.Name = new_data.Name ?? _user.Name;
                        _user.Surname = new_data.Surname ?? _user.Surname;
                        _user.PhoneNumber = new_data.PhoneNumber ?? _user.PhoneNumber;
                        _user.Password = new_data.Password ?? _user.Password;


                        await context.SaveChangesAsync();

                        return Ok("User succesfully edited");
                    }
                    return BadRequest("Incorrect user Id or user is Admin");
                }
                return BadRequest("No permission to user edition");
            }

            return Unauthorized();

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

        // GET ALL USERS
        // GET: api/User/GetAll
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetUsers()
        {
            var idClaim = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id"));
            int id = int.Parse(idClaim.Value);

            var user = context.Users.Where(u => u.UserId == id).FirstOrDefault();
            if(user != null)
            {
                if((UserRoles)user.RoleId == UserRoles.Admin)
                {
                    return Ok(context.Users.Where(u => u.RoleId != (int)UserRoles.Admin));
                }
            }

            return Unauthorized();
        }

        // CHECK IF EMAIL IS IN USE
        // POST: api/User/Email/Check
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