using Microsoft.VisualStudio.Language.Proposals;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLlmAutocomplete.VisualStudioSuggestions
{
    public static class Proposals
    {
        public static ProposalCollection CollectionFromText(string gen, ITextView textView, int position)
        {
            var point = new VirtualSnapshotPoint(textView.TextSnapshot, position);
            var snap = new SnapshotSpan(point.Position, 0);
            var edit = new ProposedEdit(snap, gen);
            var edits = new List<ProposedEdit> { edit };
            var proposals = new List<Proposal>
                {
                    new Proposal("L.AI Suggestion", edits.ToImmutableArray(), point, null, ProposalFlags.MoveCaretToEnd | ProposalFlags.SingleTabToAccept, null, null, null, null, null),
                };
            return new ProposalCollection("L.AI", proposals);
        }
    }
}
