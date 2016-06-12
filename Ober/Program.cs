using System;
using System.Diagnostics;
using System.Reflection;
using CommandLine;
using Ninject;
using Nito.AsyncEx;
using Ober.Tool.Interfaces;
using Ober.Tool.Options;

namespace Ober.Tool
{
    internal class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine();
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var result = Parser.Default.ParseArguments<SubmitOptions, ShowOptions, ListOptions>(args)
                .MapResult(
                  (SubmitOptions opts) => AsyncContext.Run(() => kernel.Get<ISubmitCommand>().CreateSubmission(opts)),
                  (ShowOptions opts) => AsyncContext.Run(() => kernel.Get<IShowCommand>().ShowSubmission(opts)),
                  (ListOptions opts) => AsyncContext.Run(() => kernel.Get<IListCommand>().ListSubmissions(opts)),
                  errs => 1);
            Console.WriteLine();
            if (!Debugger.IsAttached)
                return result;
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadLine();
            return result;
        }
    }
}
