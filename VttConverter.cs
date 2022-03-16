using System;
using System.IO;
using System.Text;

namespace srt_renamer
{
    internal class VttConverter
    {
        private readonly bool verbose;

        public VttConverter(bool verbose)
        {
            this.verbose = verbose;
        }

        public void Convert(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                if (Path.GetExtension(file).ToLower() == ".vtt")
                {
                    ConvertToSrt(file);
                }
            }
        }

        private void ConvertToSrt(string filePath)
        {
            var finalfilepath = filePath.Replace(".vtt", ".srt");
            try
            {
                using (StreamReader stream = new StreamReader(filePath))
                {
                    StringBuilder output = new StringBuilder();
                    int lineNumber = 1;
                    while (!stream.EndOfStream)
                    {
                        string line = stream.ReadLine();
                        if (IsTimecode(line))
                        {
                            output.AppendLine(lineNumber.ToString());
                            lineNumber++;
                            line = line.Replace('.', ',');
                            line = DeleteCueSettings(line);
                            output.AppendLine(line);
                            bool foundCaption = false;
                            while (true)
                            {
                                if (stream.EndOfStream)
                                {
                                    if (foundCaption)
                                        break;
                                    else
                                        throw new Exception("Corrupted file: Found timecode without caption");
                                }
                                line = stream.ReadLine();
                                if (String.IsNullOrEmpty(line) || String.IsNullOrWhiteSpace(line))
                                {
                                    output.AppendLine();
                                    break;
                                }
                                foundCaption = true;
                                output.AppendLine(line);
                            }
                        }
                    }
                    using (StreamWriter outputFile = new StreamWriter(finalfilepath))
                        outputFile.Write(output);

                    // time repaire
                    string srcfilepath2 = finalfilepath;
                    string[] lines = File.ReadAllLines(srcfilepath2);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (IsTimecode(lines[i]))
                        {

                            string secdtime = lines[i].Substring(lines[i].IndexOf('>') + 2, lines[i].LastIndexOf(",") - (lines[i].IndexOf('>') + 2));
                            string newline;
                            string finalline;
                            try
                            {
                                string oldline = lines[i];

                                TimeSpan ts = new TimeSpan();
                                ts = TimeSpan.Parse(secdtime);

                                if (secdtime == ts.ToString())
                                {
                                }
                                else
                                {
                                    newline = oldline.Replace("--> ", "--> 00:");
                                    lines[i] = newline;
                                }
                                string frsttime = lines[i].Substring(0, lines[i].IndexOf(","));
                                TimeSpan ts2 = new TimeSpan();
                                ts2 = TimeSpan.Parse(frsttime);
                                if (frsttime == ts2.ToString())
                                {
                                }
                                else
                                {
                                    finalline = lines[i].Replace(lines[i], "00:" + lines[i]);
                                    lines[i] = finalline;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(ex.Message);
                            }

                        }
                    }
                    File.WriteAllLines(srcfilepath2, lines);
                }

                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }

        bool IsTimecode(string line)
        {
            return line.Contains("-->");
        }
        bool IsTimecode2(string line)
        {
            return line.Contains("->");
        }

        string DeleteCueSettings(string line)
        {
            StringBuilder output = new StringBuilder();
            foreach (char ch in line)
            {
                char chLower = Char.ToLower(ch);
                if (chLower >= 'a' && chLower <= 'z')
                {
                    break;
                }
                output.Append(ch);
            }
            return output.ToString();
        }

        private void Log(string message, params object[] args)
        {
            if (verbose)
            {
                Console.WriteLine(message, args);
            }
        }
    }
}
