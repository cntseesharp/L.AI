using L_AI.TextGeneration.Providers.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Providers.LlamaCpp.Models
{
    internal class LlamaCppGenerationResponse
    {
        [JsonProperty("results")]
        public List<Result> Results { get; set; }

        public class Result
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
