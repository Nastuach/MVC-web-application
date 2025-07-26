using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Lab6.Models;
using Lab6.Repositories;

namespace Lab6.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repo;

        public CategoryController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public IActionResult All(int n = 0, string sort = null)
        {
            var categories = _repo.GetAllCategories();

            categories = sort?.ToLower() switch
            {
                "title" => categories.OrderBy(c => c.Title),
                "description" => categories.OrderBy(c => c.Description),
                _ => categories.OrderBy(c => c.Id)
            };

            if (n > 0) categories = categories.Take(n);

            ViewData["Sort"] = sort;
            ViewData["N"] = n;

            return View(categories.ToList());
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Category category)
        {
            var exists = _repo.GetAllCategories()
                .Any(c => c.Title == category.Title
                       && c.Description == category.Description);

            if (exists)
            {
                ModelState.AddModelError("", "Категория с такими параметрами уже существует");
            }

            if (ModelState.IsValid)
            {
                _repo.AddCategory(category);
                return RedirectToAction("All");
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = _repo.GetAllCategories().FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            var exists = _repo.GetAllCategories()
                .Any(c => c.Id != category.Id
                       && c.Title == category.Title
                       && c.Description == category.Description);

            if (exists)
            {
                ModelState.AddModelError("", "Категория с такими параметрами уже существует");
            }

            if (ModelState.IsValid)
            {
                _repo.UpdateCategory(category);
                return RedirectToAction("All");
            }
            return View(category);
        }

        public IActionResult Remove(int id)
        {
            var category = _repo.GetAllCategories().FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveConfirmed(int id)
        {
            var category = _repo.GetAllCategories().FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _repo.RemoveCategory(category);
            }
            return RedirectToAction("All");
        }

        public IActionResult Details(int id)
        {
            var category = _repo.GetAllCategories()
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            category.Questions = _repo.GetQuestionsForCategory(id).ToList();

            return View(category);
        }

        public IActionResult Stat()
        {
            var categories = _repo.GetAllCategories().ToList();

            ViewData["Count"] = categories.Count;
            ViewData["Title"] = categories.Select(c => c.Title).Distinct().ToList();
            ViewData["Description"] = categories.Select(c => c.Description).Distinct().ToList();

            return View();
        }

        public IActionResult Export(int n = 0, string sort = null)
        {
            var categories = _repo.GetAllCategories();

            categories = sort?.ToLower() switch
            {
                "title" => categories.OrderBy(c => c.Title),
                "description" => categories.OrderBy(c => c.Description),
                _ => categories.OrderBy(c => c.Id)
            };

            if (n > 0) categories = categories.Take(n);

            return Json(categories.ToList());
        }
    }
}