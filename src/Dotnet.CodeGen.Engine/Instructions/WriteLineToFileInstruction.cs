using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.CodeGen.CodeGen.Instructions
{
    public class WriteLineToFileInstruction : BaseInstruction
    {
        private StreamWriter? _stream;

        public WriteLineToFileInstruction(string command) : base(command) { }

        public override Task HandleLineAsync(string line) => _stream?.WriteLineAsync(line) ?? Task.CompletedTask;

        public override Task InitializeInstructionAsync(IProcessorContext context, params string[] parameters)
        {
            _stream?.Dispose();

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
