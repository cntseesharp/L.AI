using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI
{
    public class AutocompleteData
    {
        public static AutocompleteData Last { get; set; } = null;
        
        public int Position { get; set; }
        public string Text { get; set; }
        public ITextView TextView { get; set; }
    }
}
