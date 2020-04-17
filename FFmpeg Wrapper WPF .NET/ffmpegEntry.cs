using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg_Wrapper_WPF.NET
{
    public class FfmpegEntry
    {
        public String NewName { get; set; }
        public String Start { get; set; }
        public String End { get; set; }
        public String Source { get; set; }
        public String CommandArgs { get; set; }
        public FfmpegEntry(String name,String st,String en, String src)
        {
            NewName = name;
            NewName = "\"" + NewName + "\"";
            Start = st;
            End = en;
            Source = src;
            Source = "\"" + Source + "\"";
            InitCommandArgs();
        }
        public FfmpegEntry() { }
        public void LoadFromCSVLine(String line)
        {
            String[] args = line.Split(';');
            if (args.Length < 4)
                throw new Exception("malformed input file");
            NewName = args[0];
            NewName = "\"" + NewName + "\"";
            Start = args[1];
            End = args[2];
            Source = args[3];
            Source = "\"" + Source + "\"";
            InitCommandArgs();
        }

        internal void InitCommandArgs()
        {
            CommandArgs = "-y -i " + Source + " -ss " + Start + " -to " + End + " -async 1 " + NewName;
        }

        internal string ToCSVLine()
        {
            return String.Format("{0};{1};{2};{3}",NewName,Start,End,Source).Replace("\"",String.Empty);
        }
    }
}
