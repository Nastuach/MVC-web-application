using Microsoft.AspNetCore.Mvc;
using Lab6.Repositories;
using Lab6.Models;
using System.Linq;

namespace Lab6.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}