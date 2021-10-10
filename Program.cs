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

            [Option('v', "video", Required = false, HelpText = "Regex to match video ex:- " + @"S(\d\d?).E(\d\d?)")]
            public string VideoMatch { get; set; }

            [Option('s', "srt", Required = false, HelpText = "Regex to match srt ex:- " + @"S(\d\d?).E(\d\d?)")]
            public string SrtMatch { get; set; }

            [Option("srt-group", Required = false, HelpText = "Regex group to match srt ex:- 1")]
            public int SrtGroup { get; set; }

            [Option("video-group", Required = false, HelpText = "Regex group to match srt ex:- 2")]
            public int VideoGroup { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine(
                        FiggleFonts.Standard.Render("Srt Renamer"));

            var path = Directory.GetCurrentDirectory();


            Parser.Default.ParseArguments<Options>(args)
                       .WithParsed<Options>(o =>
                       {
                           if (!string.IsNullOrEmpty(o.Path))
                           {
                               Console.WriteLine("Using the path given {0}", o.Path);
                               path = o.Path;
                           }

                           var srtRegex = new Regex(@"S(\d\d?).E(\d\d?)");
                           var videoRegex = new Regex(@"S(\d\d?).E(\d\d?)");
                           var srtGroup = 2;
                           var videoGroup = 2;

                           if (!string.IsNullOrEmpty(o.SrtMatch))
                           {
                               if(o.Verbose) Console.WriteLine("Using the regex given {0} for matching srt files", o.SrtMatch);
                               srtRegex = new Regex(@o.SrtMatch);
                           }

                           if (!string.IsNullOrEmpty(@o.VideoMatch))
                           {
                               if (o.Verbose) Console.WriteLine("Using the regex given {0} for matching video files", o.VideoMatch);
                               videoRegex = new Regex(@o.VideoMatch);
                           }

                           if(o.VideoGroup != default(int))
                           {
                               if (o.Verbose) Console.WriteLine("Using the regex group given {0} for matching video files", o.VideoGroup);
                               videoGroup = o.VideoGroup;
                           }

                           if (o.SrtGroup != default(int))
                           {
                               if (o.Verbose) Console.WriteLine("Using the regex group given {0} for matching srt files", o.SrtGroup);
                               srtGroup = o.SrtGroup;
                           }

                           var scanner = new SubTitleScanner(o.Verbose);
                           scanner.RenameAll(videoRegex, srtRegex, path, srtGroup, videoGroup);

                           Console.WriteLine("Parsing done on files in {0}", path);

                       })
                       .WithNotParsed(o =>
                       {
                           Console.WriteLine("Could not parse the arguments. Use --help to get all arguments");
                       });
        }

    }
}
