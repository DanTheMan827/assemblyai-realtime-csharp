// See https://aka.ms/new-console-template for more information
using AssemblyAI.Realtime;
using AssemblyAI.Realtime.CLI;
using Newtonsoft.Json;

if (args.Length == 0)
{
    throw new ArgumentException("No api key provided");
}

var token = await TemporaryToken.GetTemporaryTokenAsync(args[0]);

if (token != null)
{
    using var processor = new RealtimeProcessor(token);

    var builder = new TextBuilder(processor);
    builder.OnTextChanged += Builder_OnTextChanged;

    processor.OnSessionTerminated += LogEvent;
    processor.OnDisconnected += (sender, e) => Console.WriteLine("Disconnected");
    ;
    processor.OnUnknownMessage += LogEvent;

    processor.StartTranscription();
    await Task.Delay(15000);
    processor.StopTranscription();
    Console.ReadLine();
}

void Builder_OnTextChanged(TextBuilder sender, string text)
{
    Console.Clear();
    foreach (var item in sender.GetUtterances())
    {
        Console.WriteLine(item);
    };
}

void LogEvent<T>(RealtimeProcessor sender, T e)
{
    Console.WriteLine($"{typeof(T).Name}: {JsonConvert.SerializeObject(e)}");
}