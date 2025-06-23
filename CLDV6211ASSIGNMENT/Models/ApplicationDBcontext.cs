using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CLDV6211ASSIGNMENT.Models
{
    public class ApplicationDBcontext : DbContext
    {
        public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> options) : base(options) { }

        public DbSet<Venues> Venues { get; set; }
        public DbSet<Events> Eventss { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

    }
}
