using Microsoft.EntityFrameworkCore;
using Notes.Api.Data;
using Notes.Api.Domain;
using Notes.Api.Events;
using Notes.Api.Messaging;
using RabbitMQ.Client;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<NotesDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Notes")));

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMq")!)
    };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapGet("/api/notes", async (NotesDbContext db) =>
    await db.Notes.ToListAsync());

app.MapGet("/api/notes/{id:guid}", async (Guid id, NotesDbContext db) =>
    await db.Notes.FindAsync(id) is { } note ? Results.Ok(note) : Results.NotFound());

app.MapPost("/api/notes", async (CreateNoteRequest req, NotesDbContext db, IEventPublisher publisher) =>
{
    Note note;
    try
    {
        note = Note.Create(req.Title, req.Content);
    }
    catch (ArgumentException ex)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = [ex.Message]
        });
    }

    db.Notes.Add(note);
    await db.SaveChangesAsync();
    await publisher.PublishAsync(new NoteCreated(note.Id, note.Title));
    return Results.Created($"/api/notes/{note.Id}", note);
});



app.MapDelete("/api/notes/{id:guid}", async (Guid id, NotesDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();
    db.Notes.Remove(note);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

if (app.Configuration.GetValue<bool>("MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<NotesDbContext>().Database.Migrate();
}


app.Run();

public record CreateNoteRequest(string Title, string Content);

public partial class Program;
