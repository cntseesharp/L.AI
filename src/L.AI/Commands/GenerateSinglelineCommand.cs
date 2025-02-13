using Community.VisualStudio.Toolkit;
using LocalLlmAutocomplete.CodeGeneration;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;

namespace L_AI.Commands
{
    [Command("620889b3-cd41-43ff-8786-020fd2c48b28", 4132)]
    internal sealed class GenerateSinglelineCommand : BaseCommand<GenerateSinglelineCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await GenerationHelper.StartNewAndShowAsync(true);
        }
    }
}
