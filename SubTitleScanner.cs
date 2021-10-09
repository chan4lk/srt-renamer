using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace srt_renamer
{
    public class SubTitleScanner
    {
        private readonly bool verbose;

        public SubTitleScanner(bool verbose)
        {
            this.verbose = verbose;
        }

        private void Log(string message, params object[] args)
        {
            if (verbose)
            {
                Console.WriteLine(message, args);
            }
        }

        public void RenameAll(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Directory {directory} does not exist");
                return;
            }
            var directoryInfo = Directory.GetFiles(directory);
            var srtFiles = new List<string>();
            var videos = new List<string>();
            foreach (var file in directoryInfo)
            {
                if (file.EndsWith(".srt"))
                {
                    srtFiles.Add(file);
                }
                else
                {
                    videos.Add(file);
                }
            }

            foreach (var file in srtFiles)
            {
                RenameType2(file, videos);
            }
        }
        void RenameType2(string file, List<string> videos)
        {                 
            Log("matching srt file {0}", file);

            foreach (var video in videos)
            {
                Log("matching video file {0}", video);

                var matchRatio = FuzzySharp.Fuzz.PartialRatio(file, video);

                if(matchRatio > 80)
                {
                    Log("video file {0} matched to srt {1} file", video, file);
                    Move(video, file);
                    break;
                }
            }
        }

        void Move(string video, string file)
        {
            var ext = Path.GetExtension(video);
            var newPath = file.Replace(".srt", ext);
            if (!File.Exists(newPath) && File.Exists(video))
            {
                File.Move(video, newPath);
                Log($"Renamed video {video} to {newPath}");
            }
        }
    }
}
