using System.Collections.Concurrent;

namespace Library.RabbitMQ;

public static class RmqEventDispatcher
{
    private static readonly ConcurrentDictionary<string, List<Func<Task>>> Handlers = new();

    public static void Register(string key, Func<Task> handler)
    {
        List<Func<Task>> list = Handlers.GetOrAdd(key, _ => new List<Func<Task>>());
        lock (list)
        {
            list.Add(handler);
        }
    }

    public static async Task DispatchAsync(string key)
    {
        if (!Handlers.TryGetValue(key, out List<Func<Task>>? list)) return;
        List<Func<Task>> handlers;
        lock (list)
        {
            handlers = new List<Func<Task>>(list);
        }

        foreach (Func<Task> handler in handlers)
        {
            await handler();
        }
    }
}
