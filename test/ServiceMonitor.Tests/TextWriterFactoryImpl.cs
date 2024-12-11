namespace ServiceMonitor.Tests;

internal sealed class FileAlreadyExistThrower : ITextWriterFactory
{
    public TextWriter Create(string path, bool overwrite)
    {
        if (!overwrite)
        {
            throw new IOException($"The file '{path}' already exists.");
        }

        // If '--force' is specified, fall back to 'StringWriterFactory.Create'
        return new StringWriterFactory().Create(path, overwrite);
    }
}

internal sealed class StringWriterFactory : ITextWriterFactory
{
    private static readonly Dictionary<string, StringWriter> _writers = [];

    public TextWriter Create(string path, bool overwrite)
    {
        var writer = new StringWriter();
        _writers.Add(path, writer);
        return writer;
    }

    public static StringWriter GetWriter(string path)
    {
        if (!_writers.TryGetValue(path, out var writer))
        {
            throw new InvalidOperationException();
        }
        return writer;
    }
}
