using Microsoft.EntityFrameworkCore;
namespace TodoBackend;

public class TodoDb: DbContext
{
    public DbSet<Todo> Todos => Set<Todo>();
    
    public TodoDb(DbContextOptions<TodoDb> options) : base(options)
    {
    }
}