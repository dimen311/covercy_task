using System;
using Microsoft.EntityFrameworkCore;
namespace covercy_task
{
   public class ApiKeyContext : DbContext
    {
        public ApiKeyContext(DbContextOptions<ApiKeyContext> options) : base(options) { }
        public DbSet<ApiKey> ApiKeys { get; set; }
    }
 
}