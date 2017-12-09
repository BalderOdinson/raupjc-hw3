using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Todo.EntityFramework;
using Todo.WebApplication.Models;
using Todo.WebApplication.Models.TodoViewModels;

namespace Todo.WebApplication.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _todoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoRepository todoRepository, UserManager<ApplicationUser> signInManager)
        {
            _todoRepository = todoRepository;
            _userManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(new IndexViewModel
            {
                TodoViewModels = (await _todoRepository.GetActiveAsync(user.Id)).Select(t =>
                    new TodoViewModel(t.Id)
                    {
                        Text = t.Text,
                        DateDue = t.DateDue
                    }).ToList(),
                Title = "Todo"
            });
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTodoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                    return Forbid("User not found.");
                var inputLabels = model.TodoLabels.Split(',').Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).Distinct().ToList();
                var labels = await _todoRepository.GetLabelsByNameAsync(inputLabels);
                inputLabels.Except(labels.Select(l => l.Value)).ToList().ForEach(tl =>
                {
                    labels.Add(new TodoItemLabel(tl));
                });
                var todoItem = new TodoItem(model.Text, Guid.NewGuid())
                {
                    DateDue = model.DateDue,
                    UserId = user.Id,
                    Labels = labels
                };
                await _todoRepository.AddAsync(todoItem);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> GetCompleted()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(new CompletedViewModel
            {
                TodoViewModels = (await _todoRepository.GetCompletedAsync(user.Id)).Select(t =>
                    new TodoViewModel(t.Id)
                    {
                        Text = t.Text,
                        DateDue = t.DateDue,
                        DateCompleted = t.DateCompleted
                    }).ToList(),
                Title = "Completed todos"
            });
        }

        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _todoRepository.MarkAsCompletedAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveFromCompleted(Guid id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _todoRepository.RemoveFromCompletedAsync(id, user.Id);
            return RedirectToAction(nameof(GetCompleted));
        }

    }
}