using System.IO;

namespace CodegenUP.CodeGen
{
    public interface IProcessorContext
    {
        string CommandPrefix { get; }
        string InputFile { get; }
        string OutputDirectory { get; }
    }
}
