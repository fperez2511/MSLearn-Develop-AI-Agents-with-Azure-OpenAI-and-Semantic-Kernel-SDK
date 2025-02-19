using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

string filePath = Path.GetFullPath("../../appsettings.json");
var config = new ConfigurationBuilder()
    .AddJsonFile(filePath)
    .Build();

// Set your values in appsettings.json
string modelId = config["modelId"]!;
string endpoint = config["endpoint"]!;
string apiKey = config["apiKey"]!;

// Create a kernel builder with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.AddOllamaChatCompletion(modelId, new Uri(endpoint));
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

// Build the kernel
var kernel = builder.Build();

// Create variables to store the prompt and travel information
string prompt = """
    You are a helpful travel guide. 
    I'm visiting {{$city}}. {{$background}}. What are some activities I should do today?
    """;
string city = "Barcelona";
string background = "I really enjoy art and dance.";

// Create the kernel function from the prompt
var activitiesFunction = kernel.CreateFunctionFromPrompt(prompt);

// Create the kernel arguments
var arguments = new KernelArguments { ["city"] = city, ["background"] = background };

// InvokeAsync on the kernel object
var result = await kernel.InvokeAsync(activitiesFunction, arguments);
Console.WriteLine(result);
