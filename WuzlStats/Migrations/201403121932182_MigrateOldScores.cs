using System.Collections.Generic;
using System.Linq;
using WuzlStats.Models;

namespace WuzlStats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MigrateOldScores : DbMigration
    {
        public override void Up()
        {
            using (var db = new Db())
            {
                var scores = db.Database.SqlQuery<ScoreMigrationHelper>("SELECT * FROM [Scores]").ToList();

                var players = new Dictionary<string, int>();
                foreach (var score in scores)
                {
                    if (!players.ContainsKey(score.TeamBlueDefensePlayer))
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [Players]([Name]) VALUES (@p0);", score.TeamBlueDefensePlayer);
                        players[score.TeamBlueDefensePlayer] = db.Database.SqlQuery<IdMigrationHelper>("SELECT [Id] FROM [Players] WHERE [Name] = @p0", score.TeamBlueDefensePlayer).First().Id;
                    }
                    if (!players.ContainsKey(score.TeamBlueOffensePlayer))
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [Players]([Name]) VALUES (@p0);", score.TeamBlueOffensePlayer);
                        players[score.TeamBlueOffensePlayer] = db.Database.SqlQuery<IdMigrationHelper>("SELECT [Id] FROM [Players] WHERE [Name] = @p0", score.TeamBlueOffensePlayer).First().Id;
                    }
                    if (!players.ContainsKey(score.TeamRedDefensePlayer))
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [Players]([Name]) VALUES (@p0);", score.TeamRedDefensePlayer);
                        players[score.TeamRedDefensePlayer] = db.Database.SqlQuery<IdMigrationHelper>("SELECT [Id] FROM [Players] WHERE [Name] = @p0", score.TeamRedDefensePlayer).First().Id;
                    }
                    if (!players.ContainsKey(score.TeamRedOffensePlayer))
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [Players]([Name]) VALUES (@p0);", score.TeamRedOffensePlayer);
                        players[score.TeamRedOffensePlayer] = db.Database.SqlQuery<IdMigrationHelper>("SELECT [Id] FROM [Players] WHERE [Name] = @p0", score.TeamRedOffensePlayer).First().Id;
                    }

                    db.Database.ExecuteSqlCommand("INSERT INTO [Games]([DateTime], [BlueScore], [RedScore]) VALUES (@p0, @p1, @p2);", score.Date, score.TeamBlueScore, score.TeamRedScore);
                    var gameId = db.Database.SqlQuery<IdMigrationHelper>("SELECT Max([Id]) AS [Id] FROM [Games];").First().Id;

                    if (score.TeamBlueDefensePlayer == score.TeamBlueOffensePlayer)
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [PlayerPositions]([PlayerId], [GameId], [Position], [Team]) VALUES (@p0, @p1, @p2, @p3);", players[score.TeamBlueOffensePlayer], gameId,
                            (int)Position.Both, (int)Team.Blue);
                        db.Database.ExecuteSqlCommand("INSERT INTO [PlayerPositions]([PlayerId], [GameId], [Position], [Team]) VALUES (@p0, @p1, @p2, @p3);", players[score.TeamRedOffensePlayer], gameId,
                            (int)Position.Both, (int)Team.Red);
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [PlayerPositions]([PlayerId], [GameId], [Position], [Team]) VALUES (@p0, @p1, @p2, @p3);", players[score.TeamBlueDefensePlayer], gameId,
                            (int)Position.Defense, (int)Team.Blue);
                        db.Database.ExecuteSqlCommand("INSERT INTO [PlayerPositions]([PlayerId], [GameId], [Position], [Team]) VALUES (@p0, @p1, @p2, @p3);", players[score.TeamBlueOffensePlayer], gameId,
                            (int)Position.Offense, (int)Team.Blue);
                        db.Database.ExecuteSqlCommand("INSERT INTO [PlayerPositions]([PlayerId], [GameId], [Position], [Team]) VALUES (@p0, @p1, @p2, @p3);", players[score.TeamRedDefensePlayer], gameId,
                            (int)Position.Defense, (int)Team.Red);
                        db.Database.ExecuteSqlCommand("INSERT INTO [PlayerPositions]([PlayerId], [GameId], [Position], [Team]) VALUES (@p0, @p1, @p2, @p3);", players[score.TeamRedOffensePlayer], gameId,
                            (int)Position.Offense, (int)Team.Red);
                    }
                }
            }

            DropTable("dbo.Scores");
        }

        public override void Down()
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

        public class ScoreMigrationHelper
        {
            public string TeamBlueOffensePlayer { get; set; }
            public string TeamBlueDefensePlayer { get; set; }
            public int TeamBlueScore { get; set; }
            public string TeamRedOffensePlayer { get; set; }
            public string TeamRedDefensePlayer { get; set; }
            public int TeamRedScore { get; set; }
            public DateTime Date { get; set; }
        }

        public class IdMigrationHelper
        {
            public int Id { get; set; }
        }
    }
}
