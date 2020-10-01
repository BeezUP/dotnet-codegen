using System;
using System.Threading.Tasks;

namespace CodegenUP.CodeGen.Instructions
{
    public class WriteLineToConsoleInstruction : BaseInstruction
    {
        public WriteLineToConsoleInstruction(string command) : base(command) { }

        public override Task InitializeInstructionAsync(IProcessorContext context, params string[] parameters) => Task.CompletedTask;

        public override Task HandleLineAsync(string line)
        {
            Console.WriteLine(line);
            return Task.CompletedTask;
        }

        public override void Dispose() { }
    }
}
