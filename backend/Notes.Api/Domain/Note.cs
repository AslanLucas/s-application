namespace Notes.Api.Domain;

public class Note
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    private Note() { }

    public static Note Create(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must not be empty.", nameof(title));

        return new Note { Id = Guid.NewGuid(), Title = title, Content = content };
    }
}
