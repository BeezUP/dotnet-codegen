using System.Threading.Tasks;

namespace CodegenUP.Instructions
{
    public class SuppressLineInstruction : BaseInstruction
    {
        public SuppressLineInstruction() : base(string.Empty) { }
        public override Task InitializeInstructionAsync(IProcessorContext context, params string[] parameters) => Task.CompletedTask;
        public override Task HandleLineAsync(string line) => Task.CompletedTask;
        public override void Dispose() { }
    }
}
