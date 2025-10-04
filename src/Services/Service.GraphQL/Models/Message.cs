namespace Service.GraphQL.Models;

public record Message(int Id, string Text, DateTime CreatedAtUtc);

/// <summary>
///     簡單的 In-Memory 儲存
/// </summary>
public static class MessageStore
{
    private static readonly List<Message> _messages = new();
    private static int _nextId = 1;

    public static IReadOnlyList<Message> Messages => _messages;

    public static Message Add(string text)
    {
        Message msg = new(_nextId++, text, DateTime.UtcNow);
        _messages.Add(msg);
        return msg;
    }
}
