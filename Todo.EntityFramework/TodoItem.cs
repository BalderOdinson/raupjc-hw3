using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.EntityFramework
{
    public class TodoItem
    {
        public Guid Id { get; set; }

        /// <summary>
        /// User id that owns this TodoItem
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public bool IsCompleted => DateCompleted.HasValue;

        public DateTime? DateCompleted { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date due. If null, no date was set by the user
        /// </summary>
        public DateTime? DateDue { get; set; }

        /// <summary>
        /// List of labels associated with TodoItem
        /// </summary>
        public List<TodoItemLabel> Labels { get; set; }

        public TodoItem()
        {
            // entity framework needs this one
            // not for use :)
        }

        public TodoItem(string text, Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            DateCreated = DateTime.UtcNow;
            Text = text;
            Labels = new List<TodoItemLabel>();
        }

        public bool MarkAsCompleted()
        {
            if (IsCompleted) return false;
            DateCompleted = DateTime.Now;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TodoItem todoItem)) return false;
            return todoItem.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
