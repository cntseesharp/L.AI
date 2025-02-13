using L_AI.CodeGeneration;
using L_AI.Commands;
using L_AI.Options;
using LocalLlmAutocomplete.VisualStudioSuggestions;
using Microsoft.VisualStudio.Language.Proposals;
using Microsoft.VisualStudio.Language.Suggestions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace L_AI.VisualStudioSuggestions
{
    [Export(typeof(ProposalSourceProvider))]
    [Export(typeof(ProposalSourceProviderBase))]
    [Name("LAI_ProposalSourceProvider")]
    [Order(Before = "InlineCSharpProposalSourceProvider")]
    [Order(Before = "IntelliCodeCSharpProposalSource")]
    [Order(Before = "Highest Priority")]
    [ContentType("text")]
    internal class ProposalSourceProvider : ProposalSourceProviderBase
    {
        [ImportingConstructor]
        internal ProposalSourceProvider(SuggestionServiceBase suggestionServiceBase) // { Microsoft.VisualStudio.IntelliCode.SuggestionService.Implementation.SuggestionService }
        {
            LAIPackage.Serilog.Information("[PS] Ctor");
        }

        public override async Task<ProposalSourceBase> GetProposalSourceAsync(ITextView view, CancellationToken cancel)
        {
            LAIPackage.Serilog.Information("[PS] Requested");
            return new ProposalSource(view);
        }
    }

    [Export(typeof(ProposalSourceBase))]
    [Name("LAI_ProposalSource")]
    [Order(Before = "InlineCSharpProposalSourceProvider")]
    [Order(Before = "Highest Priority")]
    [ContentType("text")]
    internal class ProposalSource : ProposalSourceBase
    {
        private readonly ITextView _textView;

        private const int _minDelayBeforeRequests = 500;

        public ProposalSource(ITextView textView)
        {
            this._textView = textView;
        }

        public override async Task<ProposalCollectionBase> RequestProposalsAsync(VirtualSnapshotPoint caret, CompletionState completionState, ProposalScenario scenario, char triggeringCharacter, CancellationToken cancel)
        {
            await Task.Delay(Math.Max(GeneralOptions.Instance.TimeBeforeMakingApiRequest, _minDelayBeforeRequests));
            if (cancel.IsCancellationRequested)
                return null;

            var caretPosition = caret.Position.Position;
            var suggestion = await GenerationSource.Instance.GetSuggestionAsync(_textView.TextSnapshot.GetText(), caretPosition, cancel);
            if (suggestion == null)
                return null;

            AutocompleteData.Last = new AutocompleteData { Text = suggestion, Position = caretPosition, TextView = _textView };

            return Proposals.CollectionFromText(suggestion, _textView, caretPosition);
        }
    }
}
