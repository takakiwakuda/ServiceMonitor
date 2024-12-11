// (c) Takaki Wakuda.

namespace ServiceMonitor;

internal interface ITextWriterFactory
{
    TextWriter Create(string path, bool overwrite);
}

internal sealed class StreamWriterFactory : ITextWriterFactory
{
    public TextWriter Create(string path, bool overwrite)
    {
        FileStreamOptions options = new()
        {
            Mode = overwrite ? FileMode.Create : FileMode.CreateNew,
            Access = FileAccess.Write,
            Share = FileShare.Read,
        };
        FileStream stream = new(path, options);

        return new StreamWriter(stream)
        {
            AutoFlush = true
        };
    }
}
