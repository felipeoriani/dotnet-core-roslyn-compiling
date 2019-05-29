Using Roslyn in .NET Core
-------------------------
This is a simple example of using Roslyn to compile some CSharp code from a string and load it into the currently executing `AppDomain`, and activate and invoke the code from the .Net Core scope. It is based on [this article](http://www.tugberkugurlu.com/archive/compiling-c-sharp-code-into-memory-and-executing-it-with-roslyn) and was adapted for the slightly different API surface area on .Net Core.

It uses Roslyn which is the native compiler for .Net Core. It can provide some features that are available from the lastest version of the language and the platform different from CodeDom which was limited by C# 4.0. CodeDom from System.CodeDom is available but not supported by .Net Core until now and when you try to use it it will thrown the `PlatformNotSupportedException`.
 
## Dependencies
To run this sample, you must to add dependencies for `Microsoft.CodeAnalysis.CSharp` and `System.Runtime.Loader`. 
