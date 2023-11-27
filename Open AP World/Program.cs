using Microsoft.Extensions.Configuration;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using OpenAI;
using System.Reflection;
using Spectre.Console;
using System.IO;

String textbook = File.ReadAllText("11.txt");

//----- Sets up the API Key -----\\
var builder = new ConfigurationBuilder()
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables();
var configurationRoot = builder.Build();

var key = configurationRoot.GetSection("OpenAIKey").Get<string>() ?? string.Empty;

var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey = key
});
//----------------------------------\\

// ----- Launch Screen, Welcome Message, and Prompts Question ----- \\
AnsiConsole.MarkupLine("[silver]-------------------------------------------------------------------------[/]");
AnsiConsole.MarkupLine("[steelblue1]\r\n   ____                      ___    ____     _       __           __    __\r\n  / __ \\____  ___  ____     /   |  / __ \\   | |     / /___  _____/ /___/ /\r\n / / / / __ \\/ _ \\/ __ \\   / /| | / /_/ /   | | /| / / __ \\/ ___/ / __  / \r\n/ /_/ / /_/ /  __/ / / /  / ___ |/ ____/    | |/ |/ / /_/ / /  / / /_/ /  \r\n\\____/ .___/\\___/_/ /_/  /_/  |_/_/         |__/|__/\\____/_/  /_/\\__,_/   \r\n    /_/                                                                   \r\n[/]");
AnsiConsole.MarkupLine("[silver]-------------------------------------------------------------------------[/]");
Console.WriteLine("");
AnsiConsole.MarkupLine("[steelblue1]Welcome to Open AP World! This is a tool that allows you to ask questions about AP World History and get answers from a textbook. This tool is powered by OpenAI's GPT-3.5 Turbo. Please enter your question below.[/]");
Console.WriteLine("");
AnsiConsole.Markup("[mediumspringgreen]>>> [/]");

// Gets the question from the user
String questions = Console.ReadLine();

// ----- Gets the answer from the API ----- \\
var completionResult = openAiService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
{
    Messages = new List<ChatMessage>
    {
        new(StaticValues.ChatMessageRoles.System, textbook),
        new(StaticValues.ChatMessageRoles.System, "Keep the answers short and simple, no unnecessary text."),
        new(StaticValues.ChatMessageRoles.User, questions),
    },
    Model = Models.Gpt_3_5_Turbo_16k,
});


await foreach (var completion in completionResult)
{
    if (completion.Successful)
    {
        Console.Write(completion.Choices.First().Message.Content);
    }
    else
    {
        if (completion.Error == null)
        {
            throw new Exception("Unknown Error");
        }

        Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
    }
}