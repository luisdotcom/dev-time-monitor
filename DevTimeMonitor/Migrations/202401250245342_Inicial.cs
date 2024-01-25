namespace DevTimeMonitor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TbDailyLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Monday = c.Boolean(),
                        Tuesday = c.Boolean(),
                        Wednesday = c.Boolean(),
                        Thursday = c.Boolean(),
                        Friday = c.Boolean(),
                        Saturday = c.Boolean(),
                        Sunday = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TbUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TbUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TbErrors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Detail = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TbUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TbTrackers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Path = c.String(nullable: false),
                        ProjectName = c.String(nullable: false),
                        FileName = c.String(nullable: false),
                        CharactersTracked = c.Int(nullable: false),
                        CharactersByCopilot = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TbUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TbTrackers", "UserId", "dbo.TbUsers");
            DropForeignKey("dbo.TbErrors", "UserId", "dbo.TbUsers");
            DropForeignKey("dbo.TbDailyLogs", "UserId", "dbo.TbUsers");
            DropIndex("dbo.TbTrackers", new[] { "UserId" });
            DropIndex("dbo.TbErrors", new[] { "UserId" });
            DropIndex("dbo.TbDailyLogs", new[] { "UserId" });
            DropTable("dbo.TbTrackers");
            DropTable("dbo.TbErrors");
            DropTable("dbo.TbUsers");
            DropTable("dbo.TbDailyLogs");
        }
    }
}
