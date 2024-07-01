# L.AI

TL;DR: Host your own AI autocomplete in Visual Studio. 
#### Only supports KoboldCpp/KoboldAI and OogaBooga, support for other APIs is not guaranteed, but you can try, if they follow the same scheme as mentioned providers.

Visual Studio plugin that adds functionality to use your own AI suggestion and autocomplete provider.
The idea came about after trying out several big language models for generating C# code. One of my concerns with AI autocompletion services always was making sure my code wasn't put at risk where others could misuse or leak it. And here is the solution to it.

##### Now works with IntelliCode disabled.

## Features
- Uses VS suggestion mechanism to provide "Tab to accept" from your local AI backend.
- Adds references (Classes, Interfaces, Enums, Records, Structs) from your code to the generation prompt to give LLM a better understanding of your code (C# only).
- Built-in tokenizer and KoboldCpp tokenizers to keep generation prompt from "pushing out" your code out of context when adding the references.
- Hotkeys for manual generation requests and to force VS to show the suggestion again if you click away and your suggestion is gone.

# Examples

Singleline\
![](https://github.com/cntseesharp/L.AI/blob/main/images/generation_example_1.jpg?raw=true)

Multiline\
![](https://github.com/cntseesharp/L.AI/blob/main/images/generation_example_2.jpg?raw=true)

# What does L.AI stand for?
L.AI simply means Local Artificial Intelligence

## Installation
Download [L.AI](https://marketplace.visualstudio.com/items?itemName=cntseesharp.LAIv1) Visual Studio Extension and install it. Make sure you have an accessible KoboldCpp instance.

## How to run my local AI?

This is a quick guide for setting up KoboldCpp and DeepSeek Coder model.

### Prerequisites
My instance ran on RTX 3090 in CUDA mode, I highly advise renting compute time, if your machine struggles with LLM Inference at an acceptable rate, since suggestion generation time is gonna hurt your experience.

1. Download and setup [KoboldCpp](https://github.com/LostRuins/koboldcpp). Extension tested on version 1.54;
2. Download any GGUF quantized coding model that was trained in instruct mode from [HuggingFace](https://huggingface.co/).\
I recommend: [DeepSeek Coder GGUF](https://huggingface.co/deepseek-ai/deepseek-coder-6.7b-instruct), it shows a decent result, but requires at least 12 GB of VRAM in 8-bit quantization mode with 4096 context length. You're looking for [deepseek-coder-6.7b-instruct.Q8_0.gguf](https://huggingface.co/TheBloke/deepseek-coder-6.7B-instruct-GGUF/resolve/main/deepseek-coder-6.7b-instruct.Q8_0.gguf?download=true) (7.2 GB of disk space);\
If you opt-out for a different model - please, check if it was trained for code insertion. DeepSeek Coder has 3 special tokens for that: <｜fim▁begin｜>, <｜fim▁hole｜> and <｜fim▁end｜>;
3. Launch KoboldCpp and select your model, don't forget to set the correct Context Size, 4096 should be enough;\
![](https://github.com/cntseesharp/L.AI/blob/main/images/kobold_example.jpg?raw=true)


## Usage
Start your Visual Studio and start typing. It follows the same rules as IntelliCode.
#### Hotkeys:
[Alt+A] Generate code suggestion.\
[Alt+Z] Generate single-line code suggestion.\
[Alt+S] Show the last suggestion.

## Q&A
Q: Why can't I install it?\
A: The extension only supports Visual Studio 2022 and higher.

Q: How do I change my endpoint?\
A: Extension has its tab in the Visual Studio Options menu. Click on L.AI button in the toolbar or navigate to the Tools -> Options... -> L.AI

Q: It's installed, but not working.\
A: You can check the extension log in the %localappdata%\Microsoft\VisualStudio\LAI_Log{date}.log

Q: I think I ran into a bug, how do I report it?\
A: Please, create a new issue in this repo and attach %localappdata%\Microsoft\VisualStudio\LAI_Log{date}.log

## Known issues
- IntelliCode loves to think that it's smart and to override suggestions from this extension from time to time. But now you can disable it completely and keep the extension's functionality.

## Contributing
Please, report any encountered issues in this repository.

## Changelog
### v1.2.1
- Fixed code being trimmed on accepting the suggestion;
- Fixed multiline generation issue. Multiline is now a default option;
- Improved API requests to accommodate generation canceling;
- Competition requests are now delayed to not overwhelm the API (configurable in settings);

### v1.2.2
- Added pane to display manual generations in output.
- Added interactivity to keep users posted about the connection status.\
![connection_status.jpg](https://github.com/cntseesharp/L.AI/blob/main/images/connection_status.jpg?raw=true)
![connection_status.jpg](https://github.com/cntseesharp/L.AI/blob/main/images/output_pane.jpg?raw=true)

### v1.3.1
- Added functionality to override VS suggestions completely. Now can work without IntelliCode enabled.
- Added option to disable automated suggestion generation
- Added a new command to generate a single-line suggestion
- Reworked the Generate and Insert logic, which now generates a suggestion instead of pasting it into the document. Suggestions generated from the commands are now automatically shown as the "ghost code"

### v1.5
- Added an option to include reference types in the generation prompt. This only applies to commands (hotkeys).
What it means is:\
When you make a manual generation request and "Use analyzer" is enabled in the options - the analyzer will lookup all references in the current file (Classes, Interfaces, Enums, Records, Structs) and append their source code at the end of the prompt, giving your LLM more context about your code, and supposedly it will improve the code generation.
- Added toolbar buttons to indicate status and provide quick access to the settings.
- Refactoring for better performance
- Added tokenizer and on KoboldCpp using its own tokenizer so it only adds to the prompt code that won't "push out" the main part of the prompt.

### v1.5.1
- Fixed a small issue that didn't change the indicator color on a failed connection.

### v1.6.0
- Reworked the way suggestions are generated.
- Fixed an issue that prevented the extension to reconnect to the backend.
- Added support for the "OogaBooga Text Generation WebUI" API.
- Fixed multiple generation priority issues.

### v1.6.1
- After experimenting with LLaMa 3 it turned out that overflowing context messes up the generation beyond imaginable. The context that's sent in the generation request is now adjusted to fit in the context length.