﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodegenUP.Instructions
{
    public class WriteLineToFileInstruction : BaseInstruction
    {
        public const string EOF = "EOF";

        private StreamWriter? _stream;

        public WriteLineToFileInstruction(string command) : base(command) { }

        public override Task HandleLineAsync(string line) => _stream?.WriteLineAsync(line) ?? Task.CompletedTask;

        public override Task InitializeInstructionAsync(IProcessorContext context, params string[] parameters)
        {
            _stream?.Dispose();
            _stream = null;

            var filePath = parameters.First();

            if (filePath == EOF) return Task.CompletedTask;

            var path = Path.Combine(context.OutputDirectory, parameters.First());
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            var fileStream = File.Open(path, FileMode.Create);
            _stream = new StreamWriter(fileStream);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
