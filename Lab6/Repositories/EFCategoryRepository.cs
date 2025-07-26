using Microsoft.EntityFrameworkCore;
using Lab6.Models;
using Lab6.Data;
using System;

namespace Lab6.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly QuestionContext _context;

        public EFCategoryRepository(QuestionContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetAllCategories()
            => _context.Categories.Include(c => c.Questions);

        public Category? GetCategoryById(int id)
            => _context.Categories.Find(id);

        public bool AddCategory(Category category)
        {
            if (CategoryExists(category.Id)) return false;

            _context.Categories.Add(category);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateCategory(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool RemoveCategory(Category category)
        {
            _context.Categories.Remove(category);
            return _context.SaveChanges() > 0;
        }

        public IQueryable<Question> GetQuestionsForCategory(int categoryId)
            => _context.Questions.Where(q => q.CategoryId == categoryId);

        public bool CategoryExists(int id)
            => _context.Categories.Any(e => e.Id == id);
    }
}