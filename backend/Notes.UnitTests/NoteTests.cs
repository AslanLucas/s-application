using Notes.Api.Domain;

namespace Notes.UnitTests;

public class NoteTests
{
    [Fact]
    public void Create_WithValidData_ReturnsNote()
    {
        var note = Note.Create("Titel", "Inhalt");

        Assert.Equal("Titel", note.Title);
        Assert.Equal("Inhalt", note.Content);
        Assert.NotEqual(Guid.Empty, note.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_Throws(string? title)
    {
        Assert.Throws<ArgumentException>(() => Note.Create(title!, "Inhalt"));
    }
}
