using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todo.EntityFramework.Exceptions;

namespace Todo.EntityFramework
{
    public class TodoSqlRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context)
        {
            _context = context;
        }

        public TodoItem Get(Guid todoId, Guid userId)
        {
            var todo = _context.TodoItems.Find(todoId);
            if (todo == null)
                return null;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            return todo;
        }

        public async Task<TodoItem> GetAsync(Guid todoId, Guid userId)
        {
            var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoId);
            if (todo == null)
                return null;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            return todo;
        }

        public void Add(TodoItem todoItem)
        {
            if (_context.TodoItems.Find(todoItem.Id) != null)
                throw new DuplicateTodoItemException($"duplicate id: {todoItem.Id}");
            _context.TodoItems.Add(todoItem);
            _context.SaveChanges();
        }

        public async Task AddAsync(TodoItem todoItem)
        {
            if (await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoItem.Id) != null)
                throw new DuplicateTodoItemException($"duplicate id: {todoItem.Id}");
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        public bool Remove(Guid todoId, Guid userId)
        {
            var todo = _context.TodoItems.Find(todoId);
            if (todo == null)
                return false;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveAsync(Guid todoId, Guid userId)
        {
            var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoId);
            if (todo == null)
                return false;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public void Update(TodoItem todoItem, Guid userId)
        {
            var todo = _context.TodoItems.Find(todoItem.Id);
            if (todo == null)
            {
                Add(todoItem);
                return;
            }
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            _context.Entry(todo).CurrentValues.SetValues(todoItem);
            _context.SaveChanges();
        }

        public async Task UpdateAsync(TodoItem todoItem, Guid userId)
        {
            var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoItem.Id);
            if (todo == null)
            {
                await AddAsync(todoItem);
                return;
            }
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            _context.Entry(todo).CurrentValues.SetValues(todoItem);
            await _context.SaveChangesAsync();
        }

        public bool MarkAsCompleted(Guid todoId, Guid userId)
        {
            var todo = _context.TodoItems.Find(todoId);
            if (todo == null)
                return false;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            if(!todo.MarkAsCompleted()) return false;
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> MarkAsCompletedAsync(Guid todoId, Guid userId)
        {
            var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoId);
            if (todo == null)
                return false;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            if (!todo.MarkAsCompleted()) return false;
            await _context.SaveChangesAsync();
            return true;
        }

        public bool RemoveFromCompleted(Guid todoId, Guid userId)
        {
            var todo = _context.TodoItems.Find(todoId);
            if (todo == null)
                return false;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            if (!todo.IsCompleted) return false;
            todo.DateCompleted = null;
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveFromCompletedAsync(Guid todoId, Guid userId)
        {
            var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoId);
            if (todo == null)
                return false;
            if (todo.UserId != userId)
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            if (!todo.IsCompleted) return false;
            todo.DateCompleted = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public List<TodoItem> GetAll(Guid userId)
        {
            return _context.TodoItems.Where(t => t.UserId == userId).OrderByDescending(t => t.DateCreated).ToList();
        }

        public async Task<List<TodoItem>> GetAllAsync(Guid userId)
        {
            return await _context.TodoItems.Where(t => t.UserId == userId).OrderByDescending(t => t.DateCreated).ToListAsync();
        }

        public List<TodoItem> GetActive(Guid userId)
        {
            return _context.TodoItems.Where(t => t.UserId == userId && !t.DateCompleted.HasValue).OrderByDescending(t => t.DateCreated).ToList();
        }

        public async Task<List<TodoItem>> GetActiveAsync(Guid userId)
        {
            return await _context.TodoItems.Where(t => t.UserId == userId && !t.DateCompleted.HasValue).OrderByDescending(t => t.DateCreated).ToListAsync();
        }

        public List<TodoItem> GetCompleted(Guid userId)
        {
            return _context.TodoItems.Where(t => t.UserId == userId && t.DateCompleted.HasValue).OrderByDescending(t => t.DateCreated).ToList();
        }

        public async Task<List<TodoItem>> GetCompletedAsync(Guid userId)
        {
            return await _context.TodoItems.Where(t => t.UserId == userId && t.DateCompleted.HasValue).OrderByDescending(t => t.DateCreated).ToListAsync();
        }

        public List<TodoItem> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userId)
        {
            return _context.TodoItems.Where(filterFunction).OrderByDescending(t => t.DateCreated).ToList();
        }

        public async Task<List<TodoItem>> GetFilteredAsync(Expression<Func<TodoItem, bool>> filterFunction, Guid userId)
        {
            return await _context.TodoItems.Where(filterFunction).OrderByDescending(t => t.DateCreated).ToListAsync();
        }

        public async Task<List<TodoItemLabel>> GetLabelsByNameAsync(List<string> labelName)
        {
            return await _context.TodoItemLabels.Where(tl => labelName.Contains(tl.Value)).ToListAsync();
        }
    }
}
