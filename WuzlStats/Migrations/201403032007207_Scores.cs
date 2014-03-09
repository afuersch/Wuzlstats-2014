namespace WuzlStats.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Scores : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        Id = c.Int(false, true),
                        TeamBlueOffensePlayer = c.String(),
                        TeamBlueDefensePlayer = c.String(),
                        TeamBlueScore = c.Int(false),
                        TeamRedOffensePlayer = c.String(),
                        TeamRedDefensePlayer = c.String(),
                        TeamRedScore = c.Int(false),
                        Date = c.DateTime(false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Scores");
        }
    }
}
