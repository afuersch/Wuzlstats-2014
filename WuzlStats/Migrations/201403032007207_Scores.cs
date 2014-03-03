namespace WuzlStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Scores : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamBlueOffensePlayer = c.String(),
                        TeamBlueDefensePlayer = c.String(),
                        TeamBlueScore = c.Int(nullable: false),
                        TeamRedOffensePlayer = c.String(),
                        TeamRedDefensePlayer = c.String(),
                        TeamRedScore = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Scores");
        }
    }
}
