using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.WebApplication.Models.TodoViewModels
{
    public class TodoViewModel
    {
        public TodoViewModel()
        { }

        public TodoViewModel(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public string Text { get; set; }

        public string DateDueOffset => DateDue != null ? 
            DateDue?.Date < DateTime.UtcNow ? $"(Prošlo prije {DateDue?.Subtract(DateTime.Today).Days * -1} {(DateDue?.Subtract(DateTime.Today).Days == -1 ? "dan" : "dana")})" : 
                $"(za {DateDue?.Subtract(DateTime.Today).Days} {(DateDue?.Subtract(DateTime.Today).Days == 1 ? "dan" : "dana")})" : null;
        
        public DateTime? DateDue { get; set; }

        public DateTime? DateCompleted { get; set; }
    }
}
