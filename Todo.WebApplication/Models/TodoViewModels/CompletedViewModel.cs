using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.WebApplication.Models.TodoViewModels
{
    public class CompletedViewModel
    {
        public string Title { get; set; }
        public List<TodoViewModel> TodoViewModels { get; set; } = new List<TodoViewModel>();
    }
}
