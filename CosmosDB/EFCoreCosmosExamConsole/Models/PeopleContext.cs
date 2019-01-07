using Microsoft.EntityFrameworkCore;

namespace EFCoreCosmosExamConsole.Models
{
    public class PeopleContext : DbContext
    {
        public DbSet<Family> Families { get; set; }

        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
              "https://ryuichi111cosmos.documents.azure.com:443/",
              "ひみつひみつひみつひみつひみつひみつひみつひみつひみつひみつひみつひみつひみつひみつひみつひみつ",
              "PeopleDatabase"
          );
        }
    }
}
