using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Todo.WebApplication.Models.TodoViewModels
{
    public class AddTodoViewModel
    {
        [Required]
        public string Text { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy", ConvertEmptyStringToNull = true)]
        public DateTime? DateDue { get; set; }

        public string TodoLabels { get; set; }
    }
}
