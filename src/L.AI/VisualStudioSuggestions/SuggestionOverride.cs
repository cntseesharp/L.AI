using LocalLlmAutocomplete.VisualStudioSuggestions;
using Microsoft.VisualStudio.Language.Proposals;
using Microsoft.VisualStudio.Language.Suggestions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace L_AI.VisualStudioSuggestions
{
    /// <summary>
    /// Hijack the VS suggestion system to show our own suggestions.
    /// Because it would be too easy to just give us a way to show what we want, wouldn't it, Microsoft?
    /// </summary>
    public static class SuggestionOverride
    {
        private static readonly MethodInfo tryDisplaySuggestionAsyncType = null;
        private static readonly MethodInfo cacheProposalType = null;
        private static readonly FieldInfo suggestionManagerType = null;
        private static readonly FieldInfo sessionType = null;

        private static readonly Type generateResultType = null;
        private static readonly Type inlineCompletionsType = null;
        private static readonly Type inlineCompletionSuggestion = null;

        static SuggestionOverride()
        {
            var assembly = Assembly.Load("Microsoft.VisualStudio.IntelliCode");

            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == "GenerateResult") generateResultType = type;
                if (type.Name == "InlineCompletionsInstance") inlineCompletionsType = type;
                if (type.Name == "InlineCompletionSuggestion") inlineCompletionSuggestion = type;
            }

            cacheProposalType = inlineCompletionsType.GetMethod("CacheProposal", BindingFlags.Instance | BindingFlags.NonPublic);
            suggestionManagerType = inlineCompletionsType.GetField("_suggestionManager", BindingFlags.Instance | BindingFlags.NonPublic);
            sessionType = inlineCompletionsType.GetField("Session", BindingFlags.Instance | BindingFlags.NonPublic);
            if (suggestionManagerType != null)
            {
                tryDisplaySuggestionAsyncType = suggestionManagerType.FieldType.GetMethod("TryDisplaySuggestionAsync");
            }
        }

        public static async Task ShowAutocompleteAsync(AutocompleteData autocompleteData)
        {
            await ShowAutocompleteAsync(autocompleteData.TextView, autocompleteData.Text, autocompleteData.Position);
        }

        public static async Task ShowAutocompleteAsync(ITextView textView, string autocompleteText, int position)
        {
            if (autocompleteText == null)
                return;

            var inlineCompletionsInstance = textView.Properties.PropertyList.FirstOrDefault(x => x.Key is Type && (x.Key as Type).Name == "InlineCompletionsInstance").Value;

            var sessionInstance = sessionType.GetValue(inlineCompletionsInstance) as SuggestionSessionBase;
            if (sessionInstance != null)
            {
                await sessionInstance.DismissAsync(ReasonForDismiss.DismissedDueToInvalidProposal, new CancellationToken());
            }

            var proposalCollection = Proposals.CollectionFromText(autocompleteText, textView, position);

            var intOut = 0;
            var generateResultInstance = Activator.CreateInstance(generateResultType, new object[] { proposalCollection, null });
            try
            {
                var ctor = inlineCompletionSuggestion.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();
                var suggestions = ctor.Invoke(new object[] { new VirtualSnapshotPoint(textView.TextSnapshot, position), null, generateResultInstance, inlineCompletionsInstance, intOut });

                var suggestionManagerInstance = suggestionManagerType.GetValue(inlineCompletionsInstance);
                var newSession = await (Task<SuggestionSessionBase>)tryDisplaySuggestionAsyncType.Invoke(suggestionManagerInstance, new object[] { suggestions, null });
                if (newSession is SuggestionSessionBase suggestionSessionBase)
                {
                    cacheProposalType.Invoke(inlineCompletionsInstance, new object[] { proposalCollection.Proposals.First() });
                    sessionType.SetValue(inlineCompletionsInstance, newSession);
                    await suggestionSessionBase.DisplayProposalAsync(proposalCollection.Proposals.First(), new CancellationToken());
                }
            }
            catch (Exception ex)
            {
                LAIPackage.Serilog.Error($"[SuggestionOverride] Exception while displaying suggestion: {ex.Message}");
            }
        }
    }
}
