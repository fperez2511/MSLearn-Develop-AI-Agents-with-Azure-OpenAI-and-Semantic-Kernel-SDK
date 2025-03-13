using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

string filePath = Path.GetFullPath("../../../../appsettings.json");
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

// Create a Handlebars prompt template that will filter the user's message
// to identify their travel dates, origin, and destination.
string prompt = File.ReadAllText(Path.GetFullPath("TravelHandlebarsPrompt.yaml"));
// Create the prompt input
string input = "I want to travel from June 1 to July 22. I want to go to Greece. I live in Chicago.";

// Create the kernel arguments
var arguments = new KernelArguments 
{ 
    ["input"] = input 
};

// For prompts with multiple parameters, use the following dictionary approach:
#pragma warning disable S125 
/* 
 * var arguments = new KernelArguments(new Dictionary<string, object?>
 * {
 *   {"request","Describe this image:"},
 *   {"imageData", "data:image/png;base64,ixxxxxxxxxx="}
 * });
 */
#pragma warning restore S125 

// Create the prompt template config using handlebars format
var templateFactory = new HandlebarsPromptTemplateFactory();
#pragma warning restore S125 // Sections of code should not be commented out
var promptTemplateConfig = new PromptTemplateConfig()
{
    Template = prompt,
    TemplateFormat = "handlebars",
    Name = "FlightPrompt",
};

// Invoke the prompt function
var function = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
var response = await kernel.InvokeAsync(function, arguments);

// Return the response
Console.WriteLine(response);
