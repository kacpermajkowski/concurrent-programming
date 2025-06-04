using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

public class DiagnosticsLogger : IDisposable
{
    private readonly BlockingCollection<string> _queue = new();
    private readonly Thread _worker;
    private volatile bool _disposed = false;

    public DiagnosticsLogger(string filePath)
    {
        _worker = new Thread(() =>
        {
            using var writer = new StreamWriter(filePath, append: true, Encoding.ASCII);
            foreach (var line in _queue.GetConsumingEnumerable())
            {
                writer.WriteLine(line);
                writer.Flush();
            }
        });
        _worker.IsBackground = true;
        _worker.Start();
    }

    public void Log(string message)
    {
        if (!_disposed)
            _queue.Add(message);
    }

    public void Dispose()
    {
        _disposed = true;
        _queue.CompleteAdding();
        _worker.Join();
    }
}

// Usage example
// var logger = new DiagnosticsLogger("path_to_your_log_file.txt");
// logger.Log($"{DateTime.Now:O},{ballId},{position.x},{position.y},{velocity.x},{velocity.y}");