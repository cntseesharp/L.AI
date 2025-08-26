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

            if (inlineCompletionsType == null)
            {
                LAIPackage.Serilog.Warning("[SuggestionOverride] InlineCompletionsInstance type not found in IntelliCode assembly.");
                return;
            }

            cacheProposalType = inlineCompletionsType.GetMethod("CacheProposal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            suggestionManagerType = inlineCompletionsType.GetField("_suggestionManager", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            sessionType = inlineCompletionsType.GetField("Session", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (suggestionManagerType != null)
            {
                var smType = suggestionManagerType.FieldType;
                tryDisplaySuggestionAsyncType = smType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .FirstOrDefault(m => m.Name == "TryDisplaySuggestionAsync");
            }
        }

        private static object TryFindSuggestionManager(object inlineCompletionsInstance)
        {
            if (inlineCompletionsInstance == null) return null;

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var instanceType = inlineCompletionsInstance.GetType();

            bool HasTryDisplaySuggestionAsync(Type t)
            {
                if (t == null) return false;
                try
                {
                    return t.GetMethods(flags).Any(m => m.Name == "TryDisplaySuggestionAsync");
                }
                catch { return false; }
            }

            // Preferred: known private field
            try
            {
                if (suggestionManagerType != null)
                {
                    var manager = suggestionManagerType.GetValue(inlineCompletionsInstance);
                    if (manager != null && HasTryDisplaySuggestionAsync(manager.GetType()))
                        return manager;
                }
            }
            catch { }

            // Fallback: scan fields
            foreach (var field in instanceType.GetFields(flags))
            {
                try
                {
                    var value = field.GetValue(inlineCompletionsInstance);
                    if (value != null && HasTryDisplaySuggestionAsync(value.GetType()))
                        return value;
                }
                catch { }
            }

            // Fallback: scan properties
            foreach (var prop in instanceType.GetProperties(flags))
            {
                try
                {
                    if (!prop.CanRead) continue;
                    var value = prop.GetValue(inlineCompletionsInstance);
                    if (value != null && HasTryDisplaySuggestionAsync(value.GetType()))
                        return value;
                }
                catch { }
            }

            return null;
        }

        public static async Task ShowAutocompleteAsync(AutocompleteData autocompleteData)
        {
            await ShowAutocompleteAsync(autocompleteData.TextView, autocompleteData.Text, autocompleteData.Position);
        }

        public static async Task ShowAutocompleteAsync(ITextView textView, string autocompleteText, int position)
        {
            if (autocompleteText == null)
                return;

            // Ensure we are on the UI thread before interacting with VS suggestion/session objects
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var inlineCompletionsInstance = textView.Properties.PropertyList.FirstOrDefault(x => x.Value != null && x.Value.GetType().Name == "InlineCompletionsInstance").Value;
            if (inlineCompletionsInstance == null)
                inlineCompletionsInstance = textView.Properties.PropertyList.FirstOrDefault(x => x.Key is Type && (x.Key as Type).Name == "InlineCompletionsInstance").Value;

            if (inlineCompletionsInstance == null)
            {
                LAIPackage.Serilog.Warning("[SuggestionOverride] InlineCompletionsInstance not found on textView; skipping display.");
                return;
            }

            var sessionInstance = sessionType.GetValue(inlineCompletionsInstance) as SuggestionSessionBase;
            if (sessionInstance != null)
            {
                await sessionInstance.DismissAsync(ReasonForDismiss.DismissedDueToInvalidProposal, new CancellationToken());
            }

            var proposalCollection = Proposals.CollectionFromText(autocompleteText, textView, position);

            var intOut = 0;
            object generateResultInstance = null;
            try
            {
                var genCtors = generateResultType?.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? Array.Empty<ConstructorInfo>();
                foreach (var ctor in genCtors)
                {
                    try
                    {
                        var ps = ctor.GetParameters();
                        var args = new object[ps.Length];
                        for (int i = 0; i < ps.Length; i++)
                        {
                            var p = ps[i];
                            if (typeof(ProposalCollectionBase).IsAssignableFrom(p.ParameterType) || typeof(ProposalCollection).IsAssignableFrom(p.ParameterType))
                                args[i] = proposalCollection;
                            else if (p.ParameterType == typeof(int))
                                args[i] = intOut;
                            else if (p.ParameterType == typeof(CancellationToken))
                                args[i] = CancellationToken.None;
                            else
                                args[i] = null;
                        }
                        generateResultInstance = ctor.Invoke(args);
                        if (generateResultInstance != null) break;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                LAIPackage.Serilog.Warning($"[SuggestionOverride] GenerateResult construction failed: {ex.Message}");
            }

            try
            {
                object suggestions = null;
                var ctors = inlineCompletionSuggestion.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .OrderBy(c => c.GetParameters().Length);
                foreach (var c in ctors)
                {
                    try
                    {
                        var ps = c.GetParameters();
                        var args = new object[ps.Length];
                        for (int i = 0; i < ps.Length; i++)
                        {
                            var p = ps[i];
                            if (p.ParameterType == typeof(VirtualSnapshotPoint)) args[i] = new VirtualSnapshotPoint(textView.TextSnapshot, position);
                            else if (generateResultType != null && p.ParameterType.IsAssignableFrom(generateResultType)) args[i] = generateResultInstance;
                            else if (inlineCompletionsType != null && p.ParameterType.IsAssignableFrom(inlineCompletionsType)) args[i] = inlineCompletionsInstance;
                            else if (p.ParameterType == typeof(int)) args[i] = intOut;
                            else if (p.ParameterType == typeof(CancellationToken)) args[i] = CancellationToken.None;
                            else args[i] = null;
                        }
                        suggestions = c.Invoke(args);
                        if (suggestions != null) break;
                    }
                    catch (Exception ex)
                    {
                        LAIPackage.Serilog.Warning($"[SuggestionOverride] InlineCompletionSuggestion ctor failed: {ex.Message}");
                    }
                }

                if (suggestions == null)
                {
                    LAIPackage.Serilog.Warning("[SuggestionOverride] Could not construct InlineCompletionSuggestion; aborting show.");
                    return;
                }

                var suggestionManagerInstance = TryFindSuggestionManager(inlineCompletionsInstance);
                if (suggestionManagerInstance == null)
                {
                    LAIPackage.Serilog.Warning("[SuggestionOverride] SuggestionManager not available; aborting show.");
                    return;
                }

                SuggestionSessionBase newSession = null;
                var overloads = suggestionManagerInstance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.Name == "TryDisplaySuggestionAsync");
                foreach (var m in overloads)
                {
                    try
                    {
                        var mps = m.GetParameters();
                        object result;
                        if (mps.Length == 1)
                        {
                            result = m.Invoke(suggestionManagerInstance, new object[] { suggestions });
                        }
                        else
                        {
                            var args = new object[mps.Length];
                            args[0] = suggestions;
                            for (int i = 1; i < mps.Length; i++)
                                args[i] = mps[i].ParameterType == typeof(CancellationToken) ? (object)CancellationToken.None : null;
                            result = m.Invoke(suggestionManagerInstance, args);
                        }
                        if (result is Task<SuggestionSessionBase> t)
                        {
                            newSession = await t;
                            if (newSession != null) break;
                        }
                        else if (result is Task genericTask)
                        {
                            await genericTask;
                            // If it's Task<T>, try to get Result via reflection
                            var taskType = genericTask.GetType();
                            var resultProp = taskType.GetProperty("Result", BindingFlags.Instance | BindingFlags.Public);
                            if (resultProp != null)
                            {
                                var taskResult = resultProp.GetValue(genericTask);
                                newSession = taskResult as SuggestionSessionBase;
                                if (newSession != null) break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LAIPackage.Serilog.Warning($"[SuggestionOverride] TryDisplaySuggestionAsync invoke failed: {ex.Message}");
                    }
                }

                // Fallback: some versions may set Session field without returning it
                if (newSession == null && sessionType != null)
                {
                    try
                    {
                        newSession = sessionType.GetValue(inlineCompletionsInstance) as SuggestionSessionBase;
                    }
                    catch { }
                }

                if (newSession is SuggestionSessionBase suggestionSessionBase)
                {
                    if (cacheProposalType != null)
                        cacheProposalType.Invoke(inlineCompletionsInstance, new object[] { proposalCollection.Proposals.First() });
                    if (sessionType != null)
                        sessionType.SetValue(inlineCompletionsInstance, newSession);
                    await suggestionSessionBase.DisplayProposalAsync(proposalCollection.Proposals.First(), new CancellationToken());
                }
                else
                {
                    LAIPackage.Serilog.Warning("[SuggestionOverride] No session returned; suggestion not shown.");
                }
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? $"{ex.Message} | Inner: {ex.InnerException.Message}" : ex.Message;
                LAIPackage.Serilog.Error($"[SuggestionOverride] Exception while displaying suggestion: {msg}");
            }
        }
    }
}
