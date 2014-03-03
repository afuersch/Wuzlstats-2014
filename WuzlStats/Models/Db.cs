using System.Data.Entity;
using WuzlStats.Migrations;

namespace WuzlStats.Models
{
    public class Db : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Db, Configuration>());

        }

        public IDbSet<Score> Scores { get; set; }
    }
}