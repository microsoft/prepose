using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 


namespace PreposeGestures
{
    public class StatisticsEntryMatch
    {
        public string poseName;
        public string gestureName;
        public long timeMs;
        public long uid; 
    }

    public class StatisticsEntrySynthesize
    {
        public string poseName;
        public string gestureName;
        public long timeMs;
        public long uid; 
    }

    public static class GestureStatistics
    {
        public static List<StatisticsEntryMatch> matchTimes;
        public static List<StatisticsEntrySynthesize> synthTimes; 

        public static void DumpStatisticsToFile(string filename)
        {
            TextWriter writer = File.CreateText(filename);

            writer.WriteLine("GestureName,PoseName,MatchTime,MatchUID");

            foreach (var match in matchTimes)
            {
                writer.WriteLine("{0},{1},{2},{3}", match.gestureName, match.poseName, match.timeMs, match.uid);
            }
            writer.Flush();

            writer.WriteLine();
            writer.WriteLine("GestureName,PoseName,SynthTime,MatchUID");

            foreach (var syn in synthTimes)
            {
                writer.WriteLine("{0},{1},{2},{3}", syn.gestureName, syn.poseName, syn.timeMs, syn.uid);
            }
            writer.Flush();
        }
    }
}
