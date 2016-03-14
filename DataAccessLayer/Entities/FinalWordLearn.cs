using System.Data.Entity;

namespace DataAccessLayer.Entities
{
    public class FinalWordLearn : DbContext
    {
        public FinalWordLearn() : base("Name=DefaultConnection")
        {
            
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<WordSuite> WordSuites { get; set; }
        public DbSet<LearningWords> LearningWords { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //for class Translation to make association with two Items
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Translation>()
                .HasOptional(b => b.OriginalItem)
                .WithMany(a => a.OriginalItemFor)
                .HasForeignKey(b => b.OriginalItemId);

            modelBuilder.Entity<Translation>()
                .HasOptional(b => b.TranslationItem)
                .WithMany(a => a.TranslationalItemFor)
                .HasForeignKey(b => b.TranslationItemId);

            modelBuilder.Entity<Translation>()
                .HasOptional(b => b.Teacher)
                .WithMany(a => a.Translations)
                .HasForeignKey(b => b.CreatorId);
            
            modelBuilder.Entity<Progress>()
                .HasOptional(p => p.Teacher)
                .WithMany(a => a.ForTeacher)
                .HasForeignKey(b => b.TeacherId);
            modelBuilder.Entity<WordSuite>()
                .HasOptional(p => p.Creater)
                .WithMany(a => a.Wordsuites)
                .HasForeignKey(b => b.UserId);
            
            //modelBuilder.Entity<Person>().HasOptional(p => p.Id).WithRequired(pi=>);
        }
    }
}
