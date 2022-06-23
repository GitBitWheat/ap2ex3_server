using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ap2ex3_server.Models;

namespace ap2ex3_server.Data
{
    public class ap2ex3_serverContext : DbContext
    {
        public ap2ex3_serverContext (DbContextOptions<ap2ex3_serverContext> options)
            : base(options)
        {
        }

        public DbSet<ap2ex3_server.Models.User>? User { get; set; }

        public DbSet<ap2ex3_server.Models.Contact>? Contact { get; set; }

        public DbSet<ap2ex3_server.Models.Message>? Message { get; set; }
    }
}
