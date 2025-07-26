using System.Collections.Generic;
using Lab6.Models;

namespace Lab6.Repositories
{
    public interface IQuestionRepository
    {
        IQueryable<Question> GetAllQuestions();
        bool AddQuestion(Question obj);
        bool RemoveQuestion(Question obj);
        bool UpdateQuestion(Question obj);

        IQueryable<Category> GetCategories();
        bool AddCategory(Category category);
        bool RemoveCategory(Category category);
        bool UpdateCategory(Category category);
    }
}