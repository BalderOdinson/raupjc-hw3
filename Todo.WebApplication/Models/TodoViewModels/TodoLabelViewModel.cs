using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.WebApplication.Models.TodoViewModels
{
    public class TodoLabelViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
