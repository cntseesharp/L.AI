using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.Options
{
    internal class GeneralOptions : OptionsBaseModel<GeneralOptions>
    {
        private static GeneralOptions _settingsInstance;
        public static GeneralOptions Instance => _settingsInstance ?? (_settingsInstance = GeneralOptions.GetLiveInstanceAsync().Result);

        [Category("Generation")]
        [DisplayName("AI Endpoint")]
        [Description("Tested on KoboldCpp")]
        [DefaultValue("http://127.0.0.1:5001")]
        public string ApiBaseEndpoint { get; set; } = "http://127.0.0.1:5001";

        [Category("Generation")]
        [DisplayName("Top P")]
        [Description("Lower for more predictable autocomplete. Highier values - more \"creative\" the AI is.")]
        [DefaultValue(0.3f)]
        public float TopP { get; set; } = 0.3f;

        [Category("Generation")]
        [DisplayName("Text begin token")]
        [Description("Will be put at the beginning of the prompt (Default for deepseek-coder)")]
        [DefaultValue("<｜fim▁begin｜>")]
        public string StartToken { get; set; } = "<｜fim▁begin｜>";

        [Category("Generation")]
        [DisplayName("Text autocomplete token")]
        [Description("Mark point of autocompletion (Default for deepseek-coder)")]
        [DefaultValue("<｜fim▁hole｜>")]
        public string HoleToken { get; set; } = "<｜fim▁hole｜>";

        [Category("Generation")]
        [DisplayName("Text end token")]
        [Description("Will be put at the end of the prompt (Default for deepseek-coder)")]
        [DefaultValue("<｜fim▁end｜>")]
        public string EndToken { get; set; } = "<｜fim▁end｜>";

        [Category("Generation")]
        [DisplayName("Max response length")]
        [Description("If Singleline is disable - here you can adjust response length in tokens")]
        [DefaultValue(100)]
        public int MaxLength { get; set; } = 100;

        [Category("Generation")]
        [DisplayName("Stop sequence")]
        [Description("Additional stop sequences to abort generation (for example ).")]
        [DefaultValue(new[] { "using System", "//Q:" })]
        public string[] StopSequence { get; set; } = new []{ "using System", "//Q:" };

        //
        //      Suggestions
        //

        [Category("Suggestions")]
        [DisplayName("Automatic suggestions enabled")]
        [Description("Choose how to treat IntelliCode-style suggestion. When disabled - will only suggest an autocomplete if prompted via hotkey or in from Tools commands.")]
        [DefaultValue(true)]
        public bool AutocompleteEnabled { get; set; } = true;

        [Category("Suggestions")]
        [DisplayName("Delay before making API request")]
        [Description("When set too low it can overload API and make a long queue of generation request. Handle with care. Min value of 500ms will be forced.")]
        [DefaultValue(1000)]
        public int TimeBeforeMakingApiRequest { get; set; } = 1000;

        [Category("Suggestions")]
        [DisplayName("Use analyzer")]
        [Description("ONLY APPLIES TO COMMANDS (Alt+A, Alt+Z). Uses VS analyzer to include code from used classes/enums/interfaces/records. Will not exceed context limit. \nHandle with care, can make inference much longer.")]
        [DefaultValue(true)]
        public bool UseAnalyzer { get; set; } = true;
    }
}
