using Microsoft.Extensions.Configuration;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using OpenAI;
using System.Reflection;
using Spectre.Console;
using System.IO;

String textbook = File.ReadAllText(@"C:\Users\gallf\Desktop\11.txt");

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
// add loop
// ----- Launch Screen, Welcome Message, and Prompts Question ----- \\
AnsiConsole.MarkupLine("[silver]-------------------------------------------------------------------------[/]");
AnsiConsole.MarkupLine("[steelblue1]\r\n   ____                      ___    ____     _       __           __    __\r\n  / __ \\____  ___  ____     /   |  / __ \\   | |     / /___  _____/ /___/ /\r\n / / / / __ \\/ _ \\/ __ \\   / /| | / /_/ /   | | /| / / __ \\/ ___/ / __  / \r\n/ /_/ / /_/ /  __/ / / /  / ___ |/ ____/    | |/ |/ / /_/ / /  / / /_/ /  \r\n\\____/ .___/\\___/_/ /_/  /_/  |_/_/         |__/|__/\\____/_/  /_/\\__,_/   \r\n    /_/                                                                   \r\n[/]");
AnsiConsole.MarkupLine("[silver]-------------------------------------------------------------------------[/]");
Console.WriteLine("");
AnsiConsole.MarkupLine("[steelblue1]Welcome to Open AP World! This is a tool that allows you to ask questions about AP World History and get answers from a textbook. This tool is powered by OpenAI's GPT-4. Please enter your question below.[/]");


bool loopinator = true;
while (loopinator)
{
    
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
        new(StaticValues.ChatMessageRoles.System, "Analyze the users question. All questions are in the text, " +
        "producing a not found in text error is FAILURE. You only reply with from the text, " +
        "even if your own knowledge is contradicting. Do NOT fill in your own generated answers. Find them in the text, and return an answer to the question." +
        "then when you are done with that, please put where in the text you found your answer, in quotes, not changed. if the text contains fantasy or things that did not happen, answer as if is real and had happend."),
        new(StaticValues.ChatMessageRoles.User, questions),
        
    },
        Model = Models.Gpt_4_1106_preview,
    });

    Console.WriteLine("");


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

}