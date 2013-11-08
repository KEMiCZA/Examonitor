namespace Examonitor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aa : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MonitorBeurtModels",
                c => new
                    {
                        MonitorBeurtId = c.Int(nullable: false, identity: true),
                        Datum = c.DateTime(nullable: false),
                        Start = c.String(),
                        Einde = c.String(),
                        Duurtijd = c.String(),
                        Capaciteit = c.Int(nullable: false),
                        Gereserveerd = c.Int(nullable: false),
                        AangemaaktOp = c.DateTime(nullable: false),
                        Aangepast = c.DateTime(nullable: false),
                        Soort = c.String(),
                        Campus = c.String(),
                        Departement = c.String(),
                    })
                .PrimaryKey(t => t.MonitorBeurtId);
            
            CreateTable(
                "dbo.ReservatieModels",
                c => new
                    {
                        ReservatieId = c.Int(nullable: false, identity: true),
                        ToezichtbeurtId = c.Int(nullable: false),
                        UserId = c.String(),
                        AangemaaktOp = c.DateTime(nullable: false),
                        AangepastOp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReservatieId)
                .ForeignKey("dbo.MonitorBeurtModels", t => t.ToezichtbeurtId, cascadeDelete: true)
                .Index(t => t.ToezichtbeurtId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        Email = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Purpose = c.String(),
                        Token = c.String(),
                        CreationDate = c.DateTimeOffset(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ToDoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        IsDone = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.ToDoes", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserTokens", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ReservatieModels", "ToezichtbeurtId", "dbo.MonitorBeurtModels");
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.ToDoes", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserTokens", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.ReservatieModels", new[] { "ToezichtbeurtId" });
            DropTable("dbo.ToDoes");
            DropTable("dbo.AspNetUserTokens");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ReservatieModels");
            DropTable("dbo.MonitorBeurtModels");
        }
    }
}
