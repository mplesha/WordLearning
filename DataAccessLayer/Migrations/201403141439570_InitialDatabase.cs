namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Visible = c.Boolean(nullable: false),
                        LanguageId = c.Int(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Languages", t => t.LanguageId)
                .Index(t => t.UserId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false, maxLength: 30),
                        Password = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        PersonRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Progresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourceId = c.Int(nullable: false),
                        StudentId = c.Int(nullable: false),
                        TeacherId = c.Int(),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(),
                        Status = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourceId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.TeacherId)
                .Index(t => t.CourceId)
                .Index(t => t.StudentId)
                .Index(t => t.TeacherId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.LearningWords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        WordSuiteId = c.Int(nullable: false),
                        TranslationId = c.Int(nullable: false),
                        LearnedDate = c.DateTime(nullable: false),
                        Progress = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .ForeignKey("dbo.Translations", t => t.TranslationId, cascadeDelete: true)
                .ForeignKey("dbo.WordSuites", t => t.WordSuiteId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.TranslationId)
                .Index(t => t.WordSuiteId);
            
            CreateTable(
                "dbo.Translations",
                c => new
                    {
                        TranslationId = c.Int(nullable: false, identity: true),
                        PartOfSpeach = c.Int(),
                        OriginalItemId = c.Int(),
                        TranslationItemId = c.Int(),
                        CreatorId = c.Int(),
                    })
                .PrimaryKey(t => t.TranslationId)
                .ForeignKey("dbo.Items", t => t.OriginalItemId)
                .ForeignKey("dbo.Users", t => t.CreatorId)
                .ForeignKey("dbo.Items", t => t.TranslationItemId)
                .Index(t => t.OriginalItemId)
                .Index(t => t.CreatorId)
                .Index(t => t.TranslationItemId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Word = c.String(nullable: false, maxLength: 100),
                        LanguageId = c.Int(nullable: false),
                        Transcription = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Lang = c.String(nullable: false, maxLength: 30),
                        ShortName = c.String(maxLength: 5),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WordSuites",
                c => new
                    {
                        WordSuiteId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Visible = c.Boolean(nullable: false),
                        CourseId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.WordSuiteId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Languages", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.UserId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 30),
                        DateOfBirth = c.DateTime(),
                        Sex = c.Boolean(),
                        PhoneNumber = c.String(),
                        Email = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ENFORCED_NUMBER_OF_ATTEMPTS = c.Int(nullable: false),
                        ENFORCED_DELAY_BETWEEN_ATTEMPTS = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WordSuiteTranslations",
                c => new
                    {
                        WordSuite_WordSuiteId = c.Int(nullable: false),
                        Translation_TranslationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.WordSuite_WordSuiteId, t.Translation_TranslationId })
                .ForeignKey("dbo.WordSuites", t => t.WordSuite_WordSuiteId, cascadeDelete: true)
                .ForeignKey("dbo.Translations", t => t.Translation_TranslationId, cascadeDelete: true)
                .Index(t => t.WordSuite_WordSuiteId)
                .Index(t => t.Translation_TranslationId);
            
            CreateTable(
                "dbo.TagTranslations",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Translation_TranslationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Translation_TranslationId })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Translations", t => t.Translation_TranslationId, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Translation_TranslationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "Id", "dbo.Users");
            DropForeignKey("dbo.Progresses", "TeacherId", "dbo.Users");
            DropForeignKey("dbo.Students", "Id", "dbo.Users");
            DropForeignKey("dbo.Progresses", "StudentId", "dbo.Students");
            DropForeignKey("dbo.LearningWords", "WordSuiteId", "dbo.WordSuites");
            DropForeignKey("dbo.TagTranslations", "Translation_TranslationId", "dbo.Translations");
            DropForeignKey("dbo.TagTranslations", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Translations", "TranslationItemId", "dbo.Items");
            DropForeignKey("dbo.Translations", "CreatorId", "dbo.Users");
            DropForeignKey("dbo.Translations", "OriginalItemId", "dbo.Items");
            DropForeignKey("dbo.WordSuiteTranslations", "Translation_TranslationId", "dbo.Translations");
            DropForeignKey("dbo.WordSuiteTranslations", "WordSuite_WordSuiteId", "dbo.WordSuites");
            DropForeignKey("dbo.WordSuites", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.WordSuites", "UserId", "dbo.Users");
            DropForeignKey("dbo.WordSuites", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Items", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.Courses", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.LearningWords", "TranslationId", "dbo.Translations");
            DropForeignKey("dbo.LearningWords", "StudentId", "dbo.Students");
            DropForeignKey("dbo.Progresses", "CourceId", "dbo.Courses");
            DropForeignKey("dbo.Courses", "UserId", "dbo.Users");
            DropIndex("dbo.Profiles", new[] { "Id" });
            DropIndex("dbo.Progresses", new[] { "TeacherId" });
            DropIndex("dbo.Students", new[] { "Id" });
            DropIndex("dbo.Progresses", new[] { "StudentId" });
            DropIndex("dbo.LearningWords", new[] { "WordSuiteId" });
            DropIndex("dbo.TagTranslations", new[] { "Translation_TranslationId" });
            DropIndex("dbo.TagTranslations", new[] { "Tag_Id" });
            DropIndex("dbo.Translations", new[] { "TranslationItemId" });
            DropIndex("dbo.Translations", new[] { "CreatorId" });
            DropIndex("dbo.Translations", new[] { "OriginalItemId" });
            DropIndex("dbo.WordSuiteTranslations", new[] { "Translation_TranslationId" });
            DropIndex("dbo.WordSuiteTranslations", new[] { "WordSuite_WordSuiteId" });
            DropIndex("dbo.WordSuites", new[] { "LanguageId" });
            DropIndex("dbo.WordSuites", new[] { "UserId" });
            DropIndex("dbo.WordSuites", new[] { "CourseId" });
            DropIndex("dbo.Items", new[] { "LanguageId" });
            DropIndex("dbo.Courses", new[] { "LanguageId" });
            DropIndex("dbo.LearningWords", new[] { "TranslationId" });
            DropIndex("dbo.LearningWords", new[] { "StudentId" });
            DropIndex("dbo.Progresses", new[] { "CourceId" });
            DropIndex("dbo.Courses", new[] { "UserId" });
            DropTable("dbo.TagTranslations");
            DropTable("dbo.WordSuiteTranslations");
            DropTable("dbo.Settings");
            DropTable("dbo.Profiles");
            DropTable("dbo.Tags");
            DropTable("dbo.WordSuites");
            DropTable("dbo.Languages");
            DropTable("dbo.Items");
            DropTable("dbo.Translations");
            DropTable("dbo.LearningWords");
            DropTable("dbo.Students");
            DropTable("dbo.Progresses");
            DropTable("dbo.Users");
            DropTable("dbo.Courses");
        }
    }
}
