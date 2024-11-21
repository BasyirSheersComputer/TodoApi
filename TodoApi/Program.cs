using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-memory storage for to-dos
var todos = new List<Todo>();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

// Define API endpoints
app.MapGet("/todos", () => todos);

app.MapPost("/todos", (Todo newTodo) =>
{
    newTodo.Id = todos.Count + 1;
    todos.Add(newTodo);
    return Results.Created($"/todos/{newTodo.Id}", newTodo);
});

app.MapPut("/todos/{id:int}", (int id, Todo updatedTodo) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo == null)
        return Results.NotFound(new { error = "To-do not found" });

    todo.Task = updatedTodo.Task ?? todo.Task;
    todo.IsComplete = updatedTodo.IsComplete;
    todo.Name = updatedTodo.Name;
    return Results.Ok(todo);
});

app.MapDelete("/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo == null)
        return Results.NotFound(new { error = "To-do not found" });

    todos.Remove(todo);
    return Results.Ok(new { message = "To-do deleted" });
});

// Configure Swagger (Optional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
