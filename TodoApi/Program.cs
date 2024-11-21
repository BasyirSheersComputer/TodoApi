using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext and configure SQLite
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TodoDatabase")));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-memory storage for to-dos
var todos = new List<Todo>();

var app = builder.Build();

// Automatically apply database migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDb>();
    dbContext.Database.EnsureCreated(); // Ensures the database is created
}

//app.MapGet("/", () => "Hello World!");

// Define API endpoints
app.MapGet("/todos", async (TodoDb dbContext) => await dbContext.Todos.ToListAsync());

app.MapPost("/todos", async (Todo newTodo, TodoDb dbContext) =>
{
    dbContext.Todos.Add(newTodo);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/todos/{newTodo.Id}", newTodo);
});

app.MapPut("/todos/{id:int}", async (int id, Todo updatedTodo, TodoDb dbContext) =>
{
    var todo = await dbContext.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound(new { error = "To-do not found" });

    todo.Task = updatedTodo.Task ?? todo.Task;
    todo.IsComplete = updatedTodo.IsComplete;
    await dbContext.SaveChangesAsync();

    return Results.Ok(todo);
});

app.MapDelete("/todos/{id:int}", async (int id, TodoDb dbContext) =>
{
    var todo = await dbContext.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound(new { error = "To-do not found" });

    dbContext.Todos.Remove(todo);
    await dbContext.SaveChangesAsync();

    return Results.Ok(new { message = "To-do deleted" });
});

// Configure Swagger (Optional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
