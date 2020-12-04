using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodegenUP.Instructions
{
    public class CleanInstruction : BaseInstruction
    {
        public CleanInstruction(string command) : base(command) { }

        public override Task HandleLineAsync(string line) => Task.CompletedTask;

        public override Task InitializeInstructionAsync(IProcessorContext context, params string[] parameters)
        {
            foreach (var file in Directory.EnumerateFiles(context.OutputDirectory, parameters.First(), SearchOption.AllDirectories))
            {
                File.Delete(file);
            }

            return Task.CompletedTask;
        }

        public override void Dispose() { }
    }
}
