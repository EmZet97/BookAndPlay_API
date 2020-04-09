using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAndPlay_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BookNadPlay_API.Controllers
{
    
    [Route("api/")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly DatabaseContext context;

        public DefaultController(IConfiguration config, DatabaseContext context)
        {
            this.configuration = config;
            this.context = context;
        }

        //public ActionResult Index()
        //{
        //    return Redirect("https://i.pinimg.com/originals/c0/72/53/c07253c6c8f81b63c1d9b0366a91ef97.jpg");
        //}

        [HttpGet]
        [Route("Test")]
        public IEnumerable<User> GetUsers()
        {
            return context.Users;
        }
    }
}