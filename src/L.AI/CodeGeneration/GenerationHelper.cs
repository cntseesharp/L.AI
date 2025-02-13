using L_AI;
using L_AI.CodeAnalysis;
using L_AI.CodeGeneration;
using L_AI.Options;
using L_AI.TextGeneration;
using L_AI.VisualStudioSuggestions;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalLlmAutocomplete.CodeGeneration
{
    public static class GenerationHelper
    {
        public static async Task StartNewAndShowAsync(bool singleline = false)
        {
            if (GenerationSource.Instance.IsBusy) return;
            var textView = await UIHelper.GetTextViewAsync(LAIPackage.Instance);
            if (textView == null)
                return;

            var pos = textView.Caret.Position.BufferPosition.Position;
            var posOriginal = pos;
            try
            {
                var text = textView.TextSnapshot.GetText();

                var tokensForMainCode = await GenerationManager.RequestTokenCount(text);
                if (!tokensForMainCode.IsSuccessful)
                    return;

                var totalTokensCount = tokensForMainCode.Data;

                var isContextLimitExceeded = GeneralOptions.Instance.ContextLength < totalTokensCount && GeneralOptions.Instance.ContextLength > 0;
                if (isContextLimitExceeded)
                {
                    var totalDiff = totalTokensCount + 100 - GeneralOptions.Instance.ContextLength;
                    var proportional = (float)text.Length / (float)totalTokensCount;

                    // Remove enough text from top and bottom until we can fit the context
                    var cursorPositionRelative = (float)pos / (float)text.Length;
                    var totalAmountOfTextToRemove = (float)proportional * (float)totalDiff;

                    var removeFromTop = (float)cursorPositionRelative * (float)totalAmountOfTextToRemove;
                    var removeFromBottom = (1 - cursorPositionRelative) * (float)totalAmountOfTextToRemove;

                    if (cursorPositionRelative < 0.1)
                    {
                        // only remove from bottom
                        removeFromBottom += removeFromTop;
                        removeFromTop = 0;
                    }

                    if (cursorPositionRelative > 0.9)
                    {
                        // only remove from top
                        removeFromTop += removeFromBottom;
                        removeFromBottom = 0;
                    }

                    //adjusting position
                    pos = pos - (int)removeFromTop;

                    text = text.Remove((int)(text.Length - removeFromBottom), (int)removeFromBottom).Remove(0, (int)removeFromTop);
                    tokensForMainCode = await GenerationManager.RequestTokenCount(text);
                    totalTokensCount = tokensForMainCode.Data;
                }

                var extraCode = new List<string>();
                if (textView.TextBuffer.ContentType.TypeName == "CSharp" && GeneralOptions.Instance.UseAnalyzer && !isContextLimitExceeded)
                {
                    var docView = textView.ToDocumentView();
                    var allReferencesCode = await CSharp.Instance.GetAllReferenesAsTextFromPathAsync(docView.FilePath);

                    if (allReferencesCode != null)
                        foreach (var code in allReferencesCode)
                        {
                            var tokensForCode = await GenerationManager.RequestTokenCount(code);
                            if (!tokensForCode.IsSuccessful)
                                break;

                            var tokensWithAddedCode = tokensForCode.Data + totalTokensCount;
                            if (tokensWithAddedCode > GeneralOptions.Instance.ContextLength)
                                continue;

                            totalTokensCount = tokensWithAddedCode;
                            extraCode.Add(code);
                        }
                }

                var generated = await GenerationSource.Instance.GetSuggestionAsync(text, pos, default, singleline, extraCode);
                AutocompleteData.Last = new AutocompleteData { Text = generated, Position = posOriginal, TextView = textView };

                await SuggestionOverride.ShowAutocompleteAsync(AutocompleteData.Last);

                await UIHelper.WindowPane.WriteLineAsync("L.AI: Manual generation request result:");
                await UIHelper.WindowPane.WriteLineAsync(generated);
            }
            catch (Exception ex)
            {
                LAIPackage.Serilog.Error($"[GenerationHelper] Something went wrong during generation: {ex.Message}");
            }
        }
    }
}
