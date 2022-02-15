namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProjectAttachments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MappingProjectAttachment", "MappingProjectId", "dbo.MappingProject");
            DropIndex("dbo.MappingProjectAttachment", new[] { "MappingProjectId" });
            DropTable("dbo.MappingProjectAttachment");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MappingProjectAttachment",
                c => new
                    {
                        MappingProjectAttachmentId = c.Guid(nullable: false, identity: true),
                        MappingProjectId = c.Guid(nullable: false),
                        AttachmentName = c.String(maxLength: 255),
                        Description = c.String(),
                        FileBytes = c.Binary(),
                        FileName = c.String(maxLength: 255),
                        MimeType = c.String(maxLength: 127),
                        CreateBy = c.String(),
                        CreateById = c.Guid(),
                        UpdateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdateById = c.Guid(),
                    })
                .PrimaryKey(t => t.MappingProjectAttachmentId);
            
            CreateIndex("dbo.MappingProjectAttachment", "MappingProjectId");
            AddForeignKey("dbo.MappingProjectAttachment", "MappingProjectId", "dbo.MappingProject", "MappingProjectId", cascadeDelete: true);
        }
    }
}
