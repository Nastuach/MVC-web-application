using System.Text.Json;
using Lab6.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab6.Models
{
    public class QuestionContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public QuestionContext(DbContextOptions<QuestionContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>()
                .Property(q => q.AnswersJson)
                .HasColumnName("Answers");

            modelBuilder.Entity<Question>()
                .Property(q => q.BadAnswersJson)
                .HasColumnName("BadAnswers");
        }
    }
}