using CVMaker.Context.Models;
using CVMaker.Services;
using Microsoft.EntityFrameworkCore;

namespace CVMaker.Context;

public class ApplicationDbContext : DbContext
{
    public Guid UserId { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IContextAccessorService accessorService) : base(options)
    {
       UserId = accessorService.UserId; 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsMany(x => x.Education, builder => builder.ToJson());
        modelBuilder.Entity<User>().OwnsOne(x => x.ContactInfo, builder => builder.ToJson());
        modelBuilder.Entity<User>().OwnsMany(x => x.Projects, builder => builder.ToJson());
        modelBuilder.Entity<User>().OwnsMany(x => x.Experiences, builder => builder.ToJson());
    }
}