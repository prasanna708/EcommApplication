namespace EcommApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ActivityDateAndTime = c.DateTime(nullable: false),
                        Action = c.String(),
                        TableEffected = c.String(),
                        TableId = c.Int(),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DataLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableEffected = c.String(),
                        PropertyId = c.Int(),
                        PropertyEffected = c.String(),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        UserId = c.String(),
                        ActivityDateAndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ErrorLogs",
                c => new
                    {
                        ErrorId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ErrorActionMethod = c.String(),
                        ErrorCode = c.Int(),
                        ErrorMessage = c.String(),
                        StackTrace = c.String(),
                        DateAndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ErrorId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(),
                        UserId = c.String(maxLength: 128),
                        Name = c.String(),
                        Quantity = c.Int(),
                        OrderDateAndTime = c.DateTime(nullable: false),
                        TotalPrice = c.Double(),
                        Photo = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.ProductId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Double(),
                        Quantity = c.Int(),
                        Photo = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Pwd = c.String(nullable: false, maxLength: 25),
                        Role = c.String(nullable: false),
                        MobileNumber = c.Long(nullable: false),
                        Email = c.String(nullable: false),
                        Dob = c.DateTime(nullable: false),
                        Location = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.PurchaseAuditLogs",
                c => new
                    {
                        OrderNumber = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(),
                        ProductId = c.Int(),
                        UserId = c.String(),
                        ProductName = c.String(),
                        Quantity = c.Int(),
                        TotalPrice = c.Double(),
                        OrderDateAndTime = c.DateTime(nullable: false),
                        Photo = c.String(),
                    })
                .PrimaryKey(t => t.OrderNumber);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "ProductId" });
            DropTable("dbo.PurchaseAuditLogs");
            DropTable("dbo.Users");
            DropTable("dbo.Products");
            DropTable("dbo.Orders");
            DropTable("dbo.ErrorLogs");
            DropTable("dbo.DataLogs");
            DropTable("dbo.ActivityLogs");
        }
    }
}
