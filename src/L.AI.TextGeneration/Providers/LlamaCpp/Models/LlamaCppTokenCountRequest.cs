using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Providers.LlamaCpp.Models
{
    internal class LlamaCppTokenCountRequest
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; }
    }
}
