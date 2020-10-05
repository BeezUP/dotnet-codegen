using System.IO;

namespace CodegenUP
{
    public interface IProcessorContext
    {
        string CommandPrefix { get; }
        string InputFile { get; }
        string OutputDirectory { get; }
    }
}
