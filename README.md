Using Roslyn in .NET Core
-------------------------
This is a simple example of using Roslyn to compile some CSharp code from a string and load it into the currently executing `AppDomain`, and activate and invoke the code from the .Net Core scope. It is based on [this article](http://www.tugberkugurlu.com/archive/compiling-c-sharp-code-into-memory-and-executing-it-with-roslyn) and [this repo](https://github.com/joelmartinez/dotnet-core-roslyn-sample) and was adapted for the slightly different API surface area on .Net Core.
 
## Dependencies
To run this sample, you must to add dependencies for `Microsoft.CodeAnalysis.CSharp` and `System.Runtime.Loader`. 
