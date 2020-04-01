using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MutualLikes.Domain.Entities;

namespace MutualLikes.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserData> UserDatas { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
    }
}
