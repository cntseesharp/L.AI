using L_AI.CodeGeneration;
using L_AI.Options;
using Microsoft.VisualStudio.Language.Proposals;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace L_AI.VisualStudioSuggestions
{
    [Export(typeof(ProposalManagerProviderBase))]
    [Name("LAI_ProposalManagerProvider")]
    [Order(Before = "InlineCSharpProposalManagerProvider")]
    [Order(Before = "IntelliCodeCSharpProposalManager")]
    [Order(Before = "Highest Priority")]
    [ContentType("text")]
    internal class ProposalManagerProvider : ProposalManagerProviderBase
    {
        public override async Task<ProposalManagerBase> GetProposalManagerAsync(ITextView view, CancellationToken cancel)
        {
            return new ProposalManager();
        }
    }

    internal class ProposalManager : ProposalManagerBase
    {
        public override bool TryGetIsProposalPosition(VirtualSnapshotPoint caret, ProposalScenario scenario, char triggerCharacter, ref bool value)
        {
            value = false;
            if (scenario != ProposalScenario.CaretMove)
                value = GeneralOptions.Instance.AutocompleteEnabled && !GenerationSource.Instance.IsBusy;

            return value;
        }
    }
}
