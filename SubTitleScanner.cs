using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace srt_renamer
{
    public class SubTitleScanner
    {

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
                var regex = new Regex(@"S[0-9]{1,2}E[0-9]{1,2}");
                var matches = regex.Match(file);
                if (matches.Success)
                {
                    var value = matches.Value;

                    foreach (var video in videos)
                    {
                        var isVideoMatch = video.IndexOf(value);
                        if (isVideoMatch > -1)
                        {
                            Move(video, file);
                            break;
                        }
                    }
                }
                else
                {

                    renameType2(file, videos);

                }

            }
        }
        void renameType2(string file, List<string> videos)
        {
            var regex = new Regex(@"S[0-9]{1,2}E \(([0-9]{1,2})\)");
            var match = regex.Match(file);

            if (match.Success)
            {
                var episodeIdStr = match.Groups[1].Value;
                var episodeId = int.Parse(episodeIdStr);
                foreach (var video in videos)
                {
                    var regex2 = new Regex(@"E([0-9]{1,2})");
                    var match2 = regex2.Match(video);
                    if (match2.Success)
                    {
                        var episode = match2.Groups[1].Value;
                        var episodeNumber = int.Parse(episode);
                        if (episodeId == episodeNumber)
                        {
                            Move(video, file);
                            break;
                        }
                    }
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
                Console.WriteLine($"Renamed file {video} to {newPath}");
            }
        }
    }
}
