using System.Collections.Generic;
using System.Linq;
using Lab6.Models;

namespace Lab6.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        bool AddCategory(Category category);
        bool UpdateCategory(Category category);
        bool RemoveCategory(Category category);
        IQueryable<Question> GetQuestionsForCategory(int categoryId);
        bool CategoryExists(int id);
    }
}