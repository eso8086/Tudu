using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoBackend;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(options => options.UseInMemoryDatabase("Todos"));


var app = builder.Build();

app.MapPost("/todo", async (TodoDb db, Todo newTodo) =>
{
    var added = await db.Todos.AddAsync(newTodo);
    await db.SaveChangesAsync();
    return added.Entity;
});

app.MapPatch("/todo", async (TodoDb db, Dictionary<string, int> requestBody) =>
{
    int id = requestBody["id"];
    Todo? todo = await db.Todos.FirstOrDefaultAsync(t=>t.Id == id);

    if (todo is null)
    {
        return Results.NotFound();
    }
        
    todo.IsComplete = true;
    db.Todos.Update(todo);
    await db.SaveChangesAsync();
    
    return Results.Ok();
});

app.MapGet("/todo", async (TodoDb db) =>
{
    return db.Todos.ToArray();
});

app.Run();