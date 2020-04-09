using BookAndPlay_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNadPlay_API
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AccessPeriod> AccessPeriods { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}
