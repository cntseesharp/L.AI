using Community.VisualStudio.Toolkit;
using L_AI.VisualStudioSuggestions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace L_AI.Commands
{
    [Command("620889b3-cd41-43ff-8786-020fd2c48b28", 4133)]
    internal sealed class InsertGeneratedCommand : BaseCommand<InsertGeneratedCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            if (AutocompleteData.Last != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(new CancellationToken());

                AutocompleteData.Last.TextView.Caret.MoveTo(new SnapshotPoint(AutocompleteData.Last.TextView.TextSnapshot, AutocompleteData.Last.Position));
                await SuggestionOverride.ShowAutocompleteAsync(AutocompleteData.Last);
            }
            else
            {
                await VS.StatusBar.ShowMessageAsync("L.AI: Nothing to insert. Use Generate command first.");
            }
        }
    }
}
