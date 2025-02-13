using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Tokenizers
{
    public class MergeEntry
    {
        public TokenNode LeftNode { get; }
        public string MergeIdentifierString { get; }
        public double MergePrio { get; }

        public MergeEntry(TokenNode leftNode, string mergeIdentifierString, double mergePrio)
        {
            LeftNode = leftNode;
            MergeIdentifierString = mergeIdentifierString;
            MergePrio = mergePrio;
        }
    }
}
