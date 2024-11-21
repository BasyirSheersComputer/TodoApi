using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
    class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
        public DbSet<Todo> Todos { get; set; }
    }
}
