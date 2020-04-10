using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg_Wrapper_WPF.NET
{
    public class ffmpegEntry
    {
        public String newName { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public String source { get; set; }
        public String commandArgs { get; set; }
        public ffmpegEntry(String name,String st,String en, String src)
        {
            newName = name;
            newName = "\"" + newName + "\"";
            start = st;
            end = en;
            source = src;
            source = "\"" + source + "\"";
            initCommandArgs();
        }
        public ffmpegEntry() { }
        public void loadFromCSVLine(String line)
        {
            String[] args = line.Split(';');
            if (args.Length < 4)
                throw new Exception("malformed input file");
            newName = args[0];
            newName = "\"" + newName + "\"";
            start = args[1];
            end = args[2];
            source = args[3];
            source = "\"" + source + "\"";
            initCommandArgs();
        }

        internal void initCommandArgs()
        {
            commandArgs = "-y -i " + source + " -ss " + start + " -to " + end + " -async 1 " + newName;
        }

        internal string ToCSVLine()
        {
            return String.Format("{0};{1};{2};{3}",newName,start,end,source).Replace("\"",String.Empty);
        }
    }
}
