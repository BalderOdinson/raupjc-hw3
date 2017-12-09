using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.EntityFramework
{
    public class TodoDbContext : DbContext
    {
        public IDbSet<TodoItem> TodoItems { get; set; }
        public IDbSet<TodoItemLabel> TodoItemLabels { get; set; }

        public TodoDbContext(string connectionString) : base(connectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>().HasKey(t => t.Id);
            modelBuilder.Entity<TodoItem>().HasMany(t => t.Labels).WithMany(tl => tl.LabelTodoItems);

            modelBuilder.Entity<TodoItemLabel>().HasKey(tl => tl.Id);

            //Required fields are defined as attributes in models.
        }
    }
}
