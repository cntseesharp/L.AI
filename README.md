# L.AI

TL;DR: Host your own AI autocomplete in Visual Studio. 
#### Only supports KoboldCpp and KoboldAI, support for different platforms is not guaranteed.

Visual Studio plugin that adds functionality to use your own AI suggestion and autocomplete provider.
The idea came about after trying out several big language models for generating C# code. One of my concerns with AI autocompletion services always was making sure my code wasn't put at risk where others could misuse or leak it. And here is the solution to it.

# Examples

Singleline\
![](https://github.com/cntseesharp/L.AI/blob/main/images/generation_example_1.jpg?raw=true)

Multiline\
![](https://github.com/cntseesharp/L.AI/blob/main/images/generation_example_2.jpg?raw=true)

# What does L.AI stand for?
L.AI simply means LocalAI

## Installation
Download [L.AI](https://marketplace.visualstudio.com/items?itemName=cntseesharp.LAIv1) Visual Studio Extension and install it. Make sure you have an accessible KoboldCpp instance.

## How to run my local AI?

This is a quick guide for setting up KoboldCpp and DeepSeek Coder model.

### Prerequisites
My instance ran on RTX 3090 in CUDA mode, I highly advise renting compute time, if your machine struggles with LLM Inference at an acceptable rate, since suggestion generation time is gonna hurt your experience.

1. Download and setup [KoboldCpp](https://github.com/LostRuins/koboldcpp). Extension tested on version 1.54;
2. Download any GGUF quantized coding model from [HuggingFace](https://huggingface.co/). I recommend: [DeepSeek Coder GGUF](https://huggingface.co/TheBloke/deepseek-coder-6.7B-base-GGUF), it shows a decent result, but requires at least 12 GB of VRAM in 8-bit quantization mode with 4096 context length. You're looking for [deepseek-coder-6.7b-base.Q8_0.gguf](https://huggingface.co/TheBloke/deepseek-coder-6.7B-base-GGUF/resolve/main/deepseek-coder-6.7b-base.Q8_0.gguf?download=true) (7.2 GB of disk space);
3. Launch KoboldCpp and select your model, don't forget to set the correct Context Size, 4096 should be enough;
![](https://github.com/cntseesharp/L.AI/blob/main/images/kobold_example.jpg?raw=true)


## Usage
Start your Visual Studio and start typing. It follows the same rules as IntelliCode.

## Q&A
Q: Why can't I install it?\
A: The extension only supports Visual Studio 2022 and higher.

Q: How do I change my endpoint?\
A: Extension has its tab in the Visual Studio Options menu. Tools -> Options... -> L.AI

Q: It's installed, but not working.\
A: Make sure IntelliCode is enabled.

Q: I only get one line of autocomplete.\
A: This is configurable the settings.

Q: I think I ran into a bug, how do I report it?
A: Please, create a new issue in this repo and attach %localappdata%\Microsoft\VisualStudio\LAI_Log{date}.log.

## Known issues
- IntelliCode loves to think that it's smart and to override suggestions from this extension from time to time.
- Code is not formatted in the suggestion.


## Contributing
Please, report any encountered issues in this repository.

## Changelog
### v1.2.1
- Fixed code being trimmed on accepting the suggestion;
- Fixed multiline generation issue. Multiline is now a default option;
- Improved API requests to accommodate generation canceling;
- Competition requests are now delayed to not overwhelm the API (configurable in settings);