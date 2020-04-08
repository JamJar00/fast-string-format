# Fast String Format
![Build and Test](https://github.com/DarkRiftNetworking/fast-string-format/workflows/Build%20and%20Test/badge.svg) [![NuGet](https://buildstats.info/nuget/FastStringFormat)](https://www.nuget.org/packages/FastStringFormat/)

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

## Current Benchmarks
Fast String Format is currently benchmarked against a number of different string formatting methods using [BenchmarkNet](https://github.com/dotnet/BenchmarkDotNet).
```
|              Method |       Mean |     Error |     StdDev |     Median |
|-------------------- |-----------:|----------:|-----------:|-----------:|
|              Concat |   963.6 ns |  96.16 ns |   246.5 ns |   900.0 ns |
|            Addition | 1,002.5 ns | 172.08 ns |   450.3 ns |   900.0 ns |
|       StringBuilder | 2,031.6 ns | 510.40 ns | 1,464.4 ns | 1,200.0 ns |
|        StringFormat | 2,184.0 ns | 468.28 ns | 1,380.7 ns | 1,400.0 ns |
| StringInterpolation | 1,550.5 ns | 381.32 ns | 1,081.7 ns | 1,000.0 ns |
|    FastStringFormat | 1,823.7 ns | 471.92 ns | 1,369.1 ns | 1,100.0 ns |
```

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
