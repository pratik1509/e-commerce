using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/todoitems", async (TodoDb db) => 
    await db.Todos.ToListAsync()
);

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) => 
    await db.Todos.FindAsync(id)
);

app.MapPost("/todoitems", async (TodoItem todo,TodoDb db) => {
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todolistitems/{todo.Id}", todo);
});

app.MapDelete("/todoitems/{id}", async (int id,TodoDb db) => {
    if(await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});


app.Run();

