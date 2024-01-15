# L.AI
Visual Studio plugin for your own LLM autocompletion. 
The idea came about after trying out several big language models for generating C# code. One of my concerns with AI autocompletion services always was making sure my code wasn't put at risk where others could misuse or leak it. And here is the solution for it.

# What does L.AI stands for?
L.AI simply means LocalAI

## Installation
Download VS extension and install it.

## How to run my local AI?

This is a quick guide for setting up KoboldCpp and DeepSeek Coder model.

### Prerequisites
My instance ran on RTX 3090 in CUDA mode, I highly advise renting compute time, if your machine struggles with LLM Inference at an acceptable rate, since suggestion generation time is gonna hurt your experience.

1. Download and setup [KoboldCpp](https://github.com/LostRuins/koboldcpp). Extension tested on version 1.54;
2. Download any quantized coding model from HuggingFace. I recommend: [DeepSeek Coder GGUF](https://huggingface.co/TheBloke/deepseek-coder-6.7B-base-GGUF), it shows a decent result, but requires at least 16 GB of VRAM in 8-bit quantization mode. You're looking for [deepseek-coder-6.7b-base.Q8_0.gguf](https://huggingface.co/TheBloke/deepseek-coder-6.7B-base-GGUF/resolve/main/deepseek-coder-6.7b-base.Q8_0.gguf?download=true) (6.7 GB of disk space);
3. Launch KoboldCpp and load your model, don't forget to set the correct Context Size, for this example I recommend 4096;


## Usage


## Q&A
Q: Why can't I install it?\
A: The extension only supports Visual Studio 2022 and higher.

Q: How do I change my endpoint?\
A: Extension has its tab in the Visual Studio Options menu. Tools -> Options... -> L.AI

Q: I only get one line of autocomplete.\
A: This is configurable the settings.

Q: I think I ran into a bug, how do I report it?
A: Please, check your %appdata%\Microsoft\VisualStudio\17.0_{RANDOM}\ActivityLog.xml for entries "LocalLlmAutocomplete" or "LAI" and/or attach it to a new issue.

## Known issues
- IntelliCode loves to think that he's smart and to override suggestions from this extension from time to time.
- Code is not formatted in the suggestion.


## Contributing
Please, report any encountered issues in this repository.