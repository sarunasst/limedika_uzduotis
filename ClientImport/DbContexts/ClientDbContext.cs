using ClientImport.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientImport.DbContexts
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
    }
}
