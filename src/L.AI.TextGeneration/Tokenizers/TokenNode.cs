using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Tokenizers
{
    public class TokenNode
    {
        public int OrigPos { get; set; }
        public int TokenId { get; set; }
        public TokenNode Prev { get; set; }
        public TokenNode Next { get; set; }
        public double MergePrio { get; set; }
        public string MergeToString { get; set; }
        public bool Deleted { get; set; }

        public TokenNode()
        {
            OrigPos = 0;
            TokenId = 0;
            Prev = null;
            Next = null;
            MergePrio = double.MaxValue;
            MergeToString = string.Empty;
            Deleted = false;
        }

        public TokenNode(int origPos, int tokenId, TokenNode prev = null)
        {
            OrigPos = origPos;
            TokenId = tokenId;
            Prev = prev;
            Next = null;
            MergePrio = double.MaxValue;
            MergeToString = string.Empty;
            Deleted = false;
        }
    }
}
