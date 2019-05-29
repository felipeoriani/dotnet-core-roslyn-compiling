using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Runtime.Versioning;

namespace DotNetCoreRoslyn.Compiling
{
    class Program
    {
        static Action<string> Write = Console.WriteLine;

        static void Main(string[] args)
        {
            var framework = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

            Write($"OsPlatform: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
            Write($"Runtime Version {framework}");
            Write($"Machine: {Environment.MachineName}");

            Write("----");

            Write("Let's compile!");

            string codeToCompile = @"
            using System;
            namespace JustADummyCode
            {
                public class Writer
                {
                    public string Write(string message)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        string r = $""Hey, this message has been written by Roslyn. Message: '{message}'"";
                        Console.WriteLine(r);
                        return r;
                    }
                }
            }";
            
            Write("Parsing the code into the SyntaxTree");

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            string assemblyName = Path.GetRandomFileName();
            
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            var references = new List<MetadataReference>();
            references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Console.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")));

            Write("Compiling ...");

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Write("Compilation failed!");

                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    Write("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("JustADummyCode.Writer");
                    var instance = assembly.CreateInstance("JustADummyCode.Writer");
                    var meth = type.GetMember("Write").First() as MethodInfo;
                    var methodResult = meth.Invoke(instance, new[] { "Felipe Oriani" });                    
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Write("Finished");

            Console.ReadLine();
        }
    }
}

