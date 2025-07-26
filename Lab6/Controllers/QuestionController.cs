using System;
using System.Collections.Generic;
using System.Linq;
using Lab6.Models;
using Lab6.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Lab6.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _repo;

        public QuestionController(IQuestionRepository repo)
        {
            _repo = repo;
        }

        public IActionResult All(int n = 0, string sort = null)
        {
            var query = _repo.GetAllQuestions()
                .Include(q => q.Category);

            var questions = SortQuestions(query, sort);

            if (n > 0) questions = questions.Take(n);

            return View(questions.ToList());
        }
        public IActionResult Start()
        {
            ViewBag.Categories = new SelectList(_repo.GetCategories(), "Id", "Title");
            return View();
        }

        public IActionResult Add()
        {
            ViewBag.CategoryId = new SelectList(_repo.GetCategories(), "Id", "Title");
            return View(new Question
            {
                Answers = new List<string>(),
                BadAnswers = new List<string>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Question question)
        {
            if (QuestionExists(question))
            {
                ModelState.AddModelError("", "Вопрос с такими параметрами уже существует");
            }
            if (question.Answers != null && question.BadAnswers != null)
            {
                var badAnswersLower = question.BadAnswers.Select(a => a.ToLowerInvariant()).ToList();
                var duplicateAnswers = question.Answers
                    .Where(answer => badAnswersLower.Contains(answer.ToLowerInvariant()))
                    .ToList();

                if (duplicateAnswers.Any())
                {
                    ModelState.AddModelError(nameof(question.Answers),
                        $"Следующие правильные ответы не должны совпадать с неправильными: {string.Join(", ", duplicateAnswers)}");
                }
            }
            if (question.Answers?.Count == 0)
            {
                ModelState.AddModelError(nameof(question.Answers), "Добавьте хотя бы один правильный ответ!");
            }
            if (question.BadAnswers?.Count == 0)
            {
                ModelState.AddModelError(nameof(question.BadAnswers), "Добавьте хотя бы один неправильный ответ!");
            }
            if (ModelState.IsValid)
            {
                _repo.AddQuestion(question);
                return RedirectToAction("All");
            }
            ViewBag.CategoryId = new SelectList(_repo.GetCategories(), "Id", "Title", question.CategoryId);
            return View(question);
        }

        public IActionResult Edit(int id)
        {
            var question = _repo.GetAllQuestions()
                .Include(q => q.Category)
                .FirstOrDefault(q => q.Id == id);

            if (question == null) return NotFound();

            ViewBag.CategoryId = new SelectList(_repo.GetCategories(), "Id", "Title", question.CategoryId);
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Question updatedQuestion, string answers, string badAnswers)
        {
            if (id != updatedQuestion.Id) return NotFound();

            var existingQuestion = _repo.GetAllQuestions()
                .FirstOrDefault(q => q.Id == id);

            if (existingQuestion == null) return NotFound();

            if (_repo.GetAllQuestions().Any(q =>
                q.Id != updatedQuestion.Id &&
                q.Text == updatedQuestion.Text &&
                q.CategoryId == updatedQuestion.CategoryId))
            {
                ModelState.AddModelError("", "Вопрос с такими параметрами уже существует");
            }

            if (ModelState.IsValid)
            {
                UpdateQuestion(existingQuestion, updatedQuestion, answers, badAnswers);
                _repo.UpdateQuestion(existingQuestion);
                return RedirectToAction("All");
            }

            ViewBag.CategoryId = new SelectList(_repo.GetCategories(), "Id", "Title", existingQuestion.CategoryId);
            return View(existingQuestion);
        }

        public IActionResult Remove(int id)
        {
            var question = _repo.GetAllQuestions()
                .Include(q => q.Category)
                .FirstOrDefault(q => q.Id == id);

            return question == null ? NotFound() : View(question);
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveConfirmed(int id)
        {
            var question = _repo.GetAllQuestions()
                .FirstOrDefault(q => q.Id == id);

            if (question != null)
            {
                _repo.RemoveQuestion(question);
            }

            return RedirectToAction("All");
        }

        public IActionResult Details(int id)
        {
            var question = _repo.GetAllQuestions()
                .Include(q => q.Category)
                .FirstOrDefault(q => q.Id == id);

            return question == null ? NotFound() : View(question);
        }

        public IActionResult Stat()
        {
            var questions = _repo.GetAllQuestions().ToList();

            ViewData["Count"] = questions.Count;
            ViewData["Text"] = questions.Select(q => q.Text).Distinct().ToList();
            ViewData["Comment"] = questions.Select(q => q.Comment).Distinct().ToList();

            return View();
        }

        public IActionResult Export(int n = 0, string sort = null)
        {
            var questions = SortQuestions(_repo.GetAllQuestions(), sort);

            if (n > 0) questions = questions.Take(n);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var data = questions.Select(q => new {
                q.Id,
                q.Text,
                Answers = q.Answers, 
                BadAnswers = q.BadAnswers,
                Category = q.Category.Title
            });

            return File(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, options)),
                "application/json",
                $"questions_{DateTime.Now:yyyyMMddHHmmss}.json"
            );
        }

        public IActionResult Test(int n = 0, int CategoryId = 0)
        {
            IQueryable<Question> query = _repo.GetAllQuestions()
                .Include(q => q.Category); 

            if (CategoryId > 0)
            {
                query = query.Where(q => q.CategoryId == CategoryId);
            }

            var questions = query
                .OrderBy(q => Guid.NewGuid())
                .Take(n > 0 ? n : int.MaxValue)
                .ToList();

            ViewBag.N = n;
            ViewBag.CategoryId = CategoryId;
            ViewBag.Categories = new SelectList(_repo.GetCategories(), "Id", "Title");

            return View(questions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Test(Dictionary<int, string> userAnswers)
        {
            var allQuestions = _repo.GetAllQuestions()
                .Include(q => q.Category)
                .ToList();

            var results = new List<AnswerResult>();
            int correctCount = 0;

            foreach (var answer in userAnswers)
            {
                var question = allQuestions.FirstOrDefault(q => q.Id == answer.Key);
                if (question != null)
                {
                    bool isCorrect = question.Answers.Contains(answer.Value.Trim());
                    if (isCorrect) correctCount++;

                    results.Add(new AnswerResult
                    {
                        Question = question,
                        UserAnswer = answer.Value,
                        IsCorrect = isCorrect
                    });
                }
            }

            return View("TestResult", new TestResultsViewModel
            {
                Results = results,
                TotalQuestions = userAnswers.Count,
                CorrectAnswers = correctCount
            });
        }

        private bool QuestionExists(Question question)
        {
            var answersJson = JsonSerializer.Serialize(question.Answers);
            var badAnswersJson = JsonSerializer.Serialize(question.BadAnswers);

            return _repo.GetAllQuestions().Any(q =>
                q.Text == question.Text &&
                q.CategoryId == question.CategoryId &&
                q.AnswersJson == answersJson && 
                q.BadAnswersJson == badAnswersJson
            );
        }

        private void UpdateQuestion(Question existing, Question updated, string answers, string badAnswers)
        {
            existing.Text = updated.Text;
            existing.Comment = updated.Comment;
            existing.CategoryId = updated.CategoryId;

            existing.Answers = answers?
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToList() ?? new List<string>();

            existing.BadAnswers = badAnswers?
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToList() ?? new List<string>();
        }

        private IQueryable<Question> SortQuestions(IQueryable<Question> query, string sort)
        {
            return sort?.ToLower() switch
            {
                "text" => query.OrderBy(q => q.Text),
                "comment" => query.OrderBy(q => q.Comment),
                "category" => query.OrderBy(q => q.Category.Title),
                _ => query.OrderBy(q => q.Id)
            };
        }
    }
    public class AnswerResult
    {
        public Question Question { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class TestResultsViewModel
    {
        public List<AnswerResult> Results { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
    }
}