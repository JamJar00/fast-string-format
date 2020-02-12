# Fast String Format
This library is designed for a future version of [DarkRift Networking](https://github.com/DarkRiftNetworking/DarkRift) to provide very fast custom formatted log lines. It is currently very work in progress and is subject to breaking changes with no notice.

## Background
### The Idea
DarkRift will soon allow users to configure the output of their logs. For example, one could create a custom log format of the time, log type, and message using something like:
```
{Timestamp:yyyy-MM-dd hh:mm:ss} | {LogType}: {Message}
```
However if DarkRift allows customisation of log lines there will be a serious performance impact when writing logs unless such customisation is done in compiled code. Therefore, this library has been built to facilitate fast string formatting capable of formatting logs lines at speed.

There are of course already string formatting options in C#. [`string.Format`](https://docs.microsoft.com/en-us/dotnet/api/system.string.format) is great but it's slow and not easy to understand if given only the format string with no parameters. [String interpolation](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated) is wonderful to read and performs well but it's unfortunately limited to compile time configuration. Fast String Format is designed to give similar readability and performance as String Interpolation but with a similar runtime flexibility as `string.Format`.

### The Implementation
Fast String Format works by using [Expressions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expressions) to compile the format string into a `Func<string, T>`. When that `Func` is run, the entire string operation happens using calls to the very fast `string.Concat` method.

## Usage
To use Fast String Format you will currently need to compile from source. Because this is a work in progress it is not available on Nuget yet.

A simple usage would look like:
```csharp
var formatter = new FastStringFormatCompiler().Compile<DataObject>("{forename} {surname} was born {DOB::yyyy-MM-dd}. It is {likesCats} that he liked cats.");

var data = new DataObject(Forename = "Steve", Surname = "Irwin", DOB = new DateTime(1962, 9, 22), LikesCats = true);

Console.WriteLine(formatter(data));
```

This would print out:
```
Steve Irwin was born 1962-09-22. It is True that he liked cats.
```

Of course, this example is trivial. Since the parsing is entirely within the `Compile` method, the real power of Fast String Format is found when a single format string needs to be applied to thousands of different data objects.

### Using Custom Formatters
When creating the `FastStringFormatCompiler` object, one can pass in a custom `IFormatStringParser` which allows custom parsing of format strings. By default the `DefaultFormatParser` is used which currently provides similar functionality to [String Interpolation](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated).

```csharp
FastStringFormatCompiler compiler = new FastStringFormatCompiler(myCustomFormatStringParser);
```