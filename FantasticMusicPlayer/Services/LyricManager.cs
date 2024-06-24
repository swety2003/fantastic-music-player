using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FantasticMusicPlayer
{
    public class LyricManager
    {

        public List<LyricEntry> lyricEntries = new List<LyricEntry>();

        private TimeSpan offset = TimeSpan.Zero;

        private bool hasValidLyrics = false;

        public class LyricEntry
        {
            public string text;
            public string translatedText;
            public bool hasTranslation = false;
            public bool isEmpty = false;
            public TimeSpan fromTime;
        }

        public LyricManager(string lyricText)
        {
            this.processLyrics(lyricText);
        }

        private void processLyrics(string txt)
        {
            string[] lines = txt.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var item in lines)
            {
                processLyricsLine(item.Trim());
            }
            lyricEntries = lyricEntries.OrderBy(t => t.fromTime).ToList();
            if(lyricEntries.Count >= 2)
            {
                this.hasValidLyrics = true;
                cachedBegin = lyricEntries[0].fromTime;
                cachedEnd = lyricEntries[1].fromTime;
                cachedEntry = lyricEntries[0];
            }
        }

        private void processLyricsLine(string line)
        {
            Regex timespanMatcher = new Regex(@"^\d{1,2}\:\d{1,2}(\.\d{1,3})?$");
            if(line.StartsWith("[") && line.Contains("]"))
            {
                string key = line.Substring(1, line.IndexOf(']') - 1);
                if (key.ToLower().StartsWith("offset") && key.Contains(":"))
                {
                    string offsetStr = key.Substring(key.IndexOf(":")+1).Replace("+","");
                    if(double.TryParse(offsetStr,out double _offset))
                    {
                        this.offset = TimeSpan.FromMilliseconds(_offset);
                    }
                    return;
                }

                if (timespanMatcher.IsMatch(key))
                {
                    string content = line.Substring(line.IndexOf(']') + 1);
                    if(TryParseTimeSpanImpl(key,out TimeSpan beginTime))
                    {
                        LyricEntry entry = new LyricEntry();
                        entry.fromTime = beginTime;
                        if (line.Contains("/"))
                        {
                            string[] texts = content.Split('/');
                            entry.hasTranslation = true;
                            entry.text = texts[0].Trim();
                            entry.translatedText = texts[1].Trim();
                        }
                        else
                        {
                            entry.text = content.Trim();
                            if(entry.text == "")
                            {
                                entry.isEmpty = true;
                            }
                        }
                        lyricEntries.Add(entry);
                    }
                    else
                    {
                        Console.WriteLine("Bad timestamp format: "+line);
                    }
                }
            }
        }

        private bool TryParseTimeSpanImpl(string str, out TimeSpan t)
        {

            string[] timeSpliter = str.Split(':').Reverse().ToArray();
            double seconds = 0;
            if(timeSpliter.Length > 0)
            {
                if (double.TryParse(timeSpliter[0],out double sec))
                {
                    seconds += sec;
                }
                else
                {
                    t = default(TimeSpan);
                    return false;
                }
            }
            if (timeSpliter.Length > 1)
            {
                if (double.TryParse(timeSpliter[1], out double min))
                {
                    seconds += min * 60d;
                }
                else
                {
                    t = default(TimeSpan);
                    return false;
                }
            }
            if (timeSpliter.Length > 2)
            {
                if (double.TryParse(timeSpliter[2], out double hour))
                {
                    seconds += hour * 3600d;
                }
                else
                {
                    t = default(TimeSpan);
                    return false;
                }
            }

            t = TimeSpan.FromSeconds(seconds);
            return true;


        }

        private TimeSpan cachedBegin = TimeSpan.Zero;
        private TimeSpan cachedEnd = TimeSpan.Zero;
        private LyricEntry cachedEntry = null;
        public LyricEntry GetLyric(TimeSpan time)
        {
            var offsetedTime = time + offset;
            if (hasValidLyrics)
            {
                if(offsetedTime < lyricEntries[0].fromTime)
                {
                    return null;
                }
                if(offsetedTime >= lyricEntries[lyricEntries.Count - 1].fromTime)
                {
                    return lyricEntries[lyricEntries.Count - 1];
                }
                if(offsetedTime >= cachedBegin && offsetedTime < cachedEnd)
                {
                    return cachedEntry;
                }

                LyricEntry _entry = null;
                int outI = 0;
                for (int i = 0; i < lyricEntries.Count; i++)
                {
                    if(offsetedTime >= lyricEntries[i].fromTime)
                    {
                        _entry = lyricEntries[i];
                       
                    }
                    else
                    {
                        outI = i - 1;
                        break;
                    }
                }
                this.cachedEntry = _entry;
                if(_entry == null)
                {
                    return null;
                }
                this.cachedBegin = this.cachedEntry.fromTime;
                this.cachedEnd = (outI == lyricEntries.Count - 1) ? TimeSpan.MaxValue : lyricEntries[outI+1].fromTime;
            }
            return null;
        }
    }
}
