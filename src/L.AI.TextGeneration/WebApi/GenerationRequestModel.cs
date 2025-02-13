using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.WebApi
{
    public class GenerationRequestModel
    {
        public int ContextLength { get; set; }
        public string[] Stop { get; set; }
        public string Prompt { get; set; }
    }
}
