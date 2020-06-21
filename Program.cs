using System;
using System.IO;

namespace srt_renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Directory.GetCurrentDirectory();
            if(args.Length > 0 && !string.IsNullOrEmpty(args[0])){
                path = args[0];
            }
            var scanner = new SubTitleScanner();
            scanner.RenameAll(path);
            Console.WriteLine("Hello World!");
        }
    }
}
