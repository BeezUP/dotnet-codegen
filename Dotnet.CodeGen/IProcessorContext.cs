using System.IO;

namespace Dotnet.CodeGen.CodeGen
{
    public interface IProcessorContext
    {
        string CommandPrefix { get; }
        string InputFile { get; }
        string OutputDirectory { get; }
    }
}
