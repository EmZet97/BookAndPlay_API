﻿using BookAndPlay_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNadPlay_API.Models
{
    public class TokenModel
    {
        public string Token { get; set; }
        public User User { get; set; }
    }
}
