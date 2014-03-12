
namespace WuzlStats.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SinglePlayerGames : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DateTime = c.DateTime(nullable: false),
                    BlueScore = c.Int(nullable: false),
                    RedScore = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.PlayerPositions",
                c => new
                {
                    GameId = c.Int(nullable: false),
                    PlayerId = c.Int(nullable: false),
                    Position = c.Int(nullable: false),
                    Team = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.GameId, t.PlayerId })
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.GameId)
                .Index(t => t.PlayerId);

            CreateTable(
                "dbo.Players",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.PlayerPositions", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.PlayerPositions", "GameId", "dbo.Games");
            DropIndex("dbo.PlayerPositions", new[] { "PlayerId" });
            DropIndex("dbo.PlayerPositions", new[] { "GameId" });
            DropTable("dbo.Players");
            DropTable("dbo.PlayerPositions");
            DropTable("dbo.Games");
        }
    }
}
