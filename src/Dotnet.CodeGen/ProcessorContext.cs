using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dotnet.CodeGen.CodeGen
{
    class ProcessorContext : IProcessorContext
    {
        public ProcessorContext(string inputFile, string outputDirectory)
        {
            InputFile = inputFile;
            OutputDirectory = outputDirectory;
        }

        public string CommandPrefix { get; set; } = "###";

        public string InputFile { get; set; }

        public string OutputDirectory { get; set; }
    }
}
