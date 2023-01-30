using dotnet_pet_api.Models;
using Microsoft.EntityFrameworkCore;

namespace DbaseContext
{
    class PetsDb : DbContext
    {
        public PetsDb(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Categories> Categories { get; set; } = null!;
        public DbSet<Users> Users { get; set; } = null!;
    }
}