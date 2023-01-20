using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace EFModelCreateApp
{
    public class NewOne
    {
        public int Id { get; set; }
        public DateTime DatePublish { set; get; }
        public string? Author { set; get; }
    }

    [EntityTypeConfiguration(typeof(UserConfiguration))]
    public class User
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        
        //[MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }
        public int? Age { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Age)
                   .HasDefaultValue(14);
            builder.Property(u => u.Name)
                   .HasComputedColumnSql("FirstName + ' ' + LastName");
            builder.HasCheckConstraint("Age", "Age > 0 AND Age < 65");
            builder.Property(u => u.FirstName)
                   .HasMaxLength(50);

            builder.HasData(new User { Id = 1, FirstName = "Tom", LastName = "Smith", Age = 42 },
                new User { Id = 2, FirstName = "Leo", LastName = "Watson", Age = 38 });
        }
    }

    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<NewOne> News { get; set; }
        public AppContext() 
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WorkDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAddOrUpdate();
            //modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            using(AppContext context = new())
            {
                User user = new() { FirstName = "Bob", LastName = "Smith" };
                Console.WriteLine(user.Id);
                context.Users.Add(user);
                context.SaveChanges();
                Console.WriteLine(user.Id);

                context.Users.Add(new User { FirstName = "Joe", LastName = "Biden" });
                context.SaveChanges();

                //NewOne newOne = new() { Author = "Smirnov Alex" };
                //context.News.Add(newOne);
                //context.SaveChanges();

            }
        }
    }
}