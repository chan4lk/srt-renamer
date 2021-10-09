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

        public void RenameAll(Regex videoRegex, Regex srtRegex, string directory)
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
                RenameType2(videoRegex, srtRegex, file, videos);
            }
        }
        void RenameType2(Regex videoRegex, Regex srtRegex, string file, List<string> videos)
        {            
            var match = srtRegex.Match(file);

            Log("matching srt file {0}", file);

            if (match.Success)
            {
                Log("srt file {0} matched regex", file);

                var episodeIdStr = match.Groups[2].Value;
                var episodeId = int.Parse(episodeIdStr);
                foreach (var video in videos)
                {
                    Log("matching video file {0}", video);

                    var match2 = videoRegex.Match(video);
                    if (match2.Success)
                    {
                        Log("video file {0} matched regex", video);

                        var episode = match2.Groups[2].Value;
                        var episodeNumber = int.Parse(episode);
                        if (episodeId == episodeNumber)
                        {
                            Log("video file {0} matched to srt {1} file", video, file);
                            Move(video, file);
                            break;
                        }
                    }
                }
            }else
            {

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
