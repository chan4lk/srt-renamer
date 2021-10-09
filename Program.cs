using CommandLine;
using Figgle;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace srt_renamer
{
    class Program
    {
        public class Options
        {
            [Option('l', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('p', "path", Required = false, HelpText = "Path to folder of videos with subtitle files.")]
            public string Path { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine(
                        FiggleFonts.Standard.Render("Srt Renamer from @chan4lk"));

            var path = Directory.GetCurrentDirectory();


            Parser.Default.ParseArguments<Options>(args)
                       .WithParsed<Options>(o =>
                       {
                           if (!string.IsNullOrEmpty(o.Path))
                           {
                               Console.WriteLine("Using the path given {0}", o.Path);
                               path = o.Path;
                           }                          

                           var scanner = new SubTitleScanner(o.Verbose);
                           scanner.RenameAll(path);

                           Console.WriteLine("Parsing done on files in {0}", path);

                       })
                       .WithNotParsed(o =>
                       {
                           Console.WriteLine("Could not parse the arguments. Use --help to get all arguments");
                       });
        }

    }
}
