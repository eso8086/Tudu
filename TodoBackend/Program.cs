using Microsoft.EntityFrameworkCore;
using TodoBackend;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(options=>options.UseSqlite("Data Source=todos.db"));


var app = builder.Build();

app.MapGet("/todos", async (TodoDb db) =>
{
    return await db.Todos.ToArrayAsync();
});

app.MapGet("/todos/complete", async (TodoDb db) =>
{
    return await db.Todos.Where(t => t.IsComplete).ToListAsync();
});

app.MapGet("/todos/{id}", async (int id, TodoDb db) =>
{
    return await db.Todos.FindAsync(id)
        is Todo todo
        ? Results.Ok(todo) 
        : Results.NotFound();
});

app.MapPost("/todos", async (TodoDb db, Todo newTodo) =>
{
    await db.Todos.AddAsync(newTodo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{newTodo.Id}", newTodo);
});

// TODO: handle invalid inputTodo content
app.MapPut("/todos/{id}", async (int id, TodoDb db, Todo inputTodo) =>
{
    Todo? todo = await db.Todos.FindAsync(id);
    
    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;
  
    await db.SaveChangesAsync();
    
    return Results.Ok();
});

app.MapDelete("todos/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    
    return Results.NotFound();
});


app.Run();