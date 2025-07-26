using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Lab6.Models;
using System.Text.Json;

namespace Lab6.Repositories
{
    public class EFQuestionRepository : IQuestionRepository
    {
        private readonly QuestionContext _context;

        public EFQuestionRepository(QuestionContext context)
        {
            _context = context;
        }

        public IQueryable<Question> GetAllQuestions()
        {
            return _context.Questions
                .Include(q => q.Category)
                .AsQueryable();
        }

        public bool AddQuestion(Question question)
        {
            _context.Questions.Add(question);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateQuestion(Question question)
        {
            var entity = _context.Questions.Find(question.Id);
            if (entity == null) return false;

            entity.Text = question.Text;
            entity.Comment = question.Comment;
            entity.CategoryId = question.CategoryId;
            entity.Answers = question.Answers;
            entity.BadAnswers = question.BadAnswers;

            return _context.SaveChanges() > 0;
        }

        public bool RemoveQuestion(Question question)
        {
            var entity = _context.Questions.Find(question.Id);
            if (entity == null) return false;

            _context.Questions.Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public IQueryable<Category> GetCategories()
        {
            return _context.Categories.Include(c => c.Questions);
        }

        public bool AddCategory(Category category)
        {
            _context.Categories.Add(category);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateCategory(Category category)
        {
            var entity = _context.Categories.Find(category.Id);
            if (entity == null) return false;

            entity.Title = category.Title;
            entity.Description = category.Description;
            return _context.SaveChanges() > 0;
        }

        public bool RemoveCategory(Category category)
        {
            var entity = _context.Categories.Find(category.Id);
            if (entity == null) return false;

            _context.Categories.Remove(entity);
            return _context.SaveChanges() > 0;
        }
    }
}