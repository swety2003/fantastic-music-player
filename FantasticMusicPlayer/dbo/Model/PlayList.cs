using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer.dbo.Model
{
    public class PlayList
    {
        private Guid Guid { get; } = Guid.NewGuid();
        public int ID { get; set; } = 0;
        public String Name { get; set; }

        public PlayList(string name)
        {
            Name = name;
        }
        public PlayList(string name,IEnumerable<SongEntry> songs)
        {
            Name = name;
            foreach (SongEntry item in songs)
            {
                Songs.Add(item);
            }
        }
        public virtual List<SongEntry> Songs { get; set; } = new List<SongEntry>();
 
        public override bool Equals(object obj)
        {
            return obj is PlayList list &&
                   Guid.Equals(list.Guid);
        }

        public override int GetHashCode()
        {
            return -737073652 + Guid.GetHashCode();
        }
    }
}
